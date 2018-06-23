using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Persistence;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Akka.Event;
using Newtonsoft.Json;

namespace vmstats
{
    public class MetricStoreActor : ReceivePersistentActor
    {
        #region Messages
        public class UpsertMetric
        {
            public UpsertMetric(string name, SortedDictionary<long, float> metrics)
            {
                this.name = name;
                this.metrics = metrics;
            }

            public string name { get; set; }
            public SortedDictionary<long, float> metrics { get; set; }
        }
        #endregion

        #region Instance variables
        private MetricStore _metricStore;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        #endregion

        public override string PersistenceId
        {
            get
            {
                // Return the unique name for this actor as the persistence id
                return Context.Self.Path.Name;
            }
        }


        public MetricStoreActor(string vmName, string date)
        {
            _metricStore = new MetricStore(vmName, date);

            // Recover messages 
            Recover<UpsertMetric>(um => ProcessUpsertMetric(um));
            Recover<SnapshotOffer>(offer => {
                var ms = offer.Snapshot as MetricStore;
                if (ms != null) // null check
                    _metricStore = ms;
            });

            // Commands
            Command<UpsertMetric>(um => Persist(um, s => {
                _log.Debug($"Received UpserMetrics msg actor id={PersistenceId}");
                ProcessUpsertMetric(um);
                SaveSnapshot(_metricStore);
            }));

            Command<SaveSnapshotSuccess>(success => {
                // soft-delete the journal up until the sequence # at
                // which the snapshot was taken
                DeleteMessages(success.Metadata.SequenceNr);

                // Delete all previous snapshots so we only keep the latest one
                var snapSelectCrit = new SnapshotSelectionCriteria(success.Metadata.SequenceNr - 1, success.Metadata.Timestamp, 0, new DateTime(0));
                DeleteSnapshots(snapSelectCrit);
                _log.Info($"Save snapshot successful for actor id={PersistenceId}");
            });

            Command<SaveSnapshotFailure>(failure => {
                _log.Error($"ERROR: Failed to save snapshot for actor with id={PersistenceId}");
            });

            Command<Messages.BuildTransformSeries>(msg => ProcessPipeline(msg));
        }


        private void ProcessPipeline(Messages.BuildTransformSeries cmd)
        {
            // Get the metric requested in the pipeline
            Metric metric = null;
            _metricStore.metrics.TryGetValue(cmd.MetricName.ToLower(), out metric);
            if (metric != null)
            {
                // Create a start transform message and submit it to the first transform in the queue
                var msg = new Messages.TransformSeries(metric, cmd.Transforms, cmd.GroupID, cmd.ConnectionId);

                BaseTransformActor.RouteTransform(msg);
            }
            else
            {
                // ERROR the requested metric does not exist in this actor
                var json = JsonConvert.SerializeObject(_metricStore.metrics.Keys);
                _log.Error($"ERROR: Received ProcessPipeline command for a metric that does not exist. Metric requested is: {cmd.MetricName}. Available metrics are: {json}");
            }
        }

        private void ProcessUpsertMetric(UpsertMetric um)
        {
            // If the metric already existed then add any missing values to it from the received 
            // ones in the upsert message
            Metric retrievedMetric;
            if (_metricStore.metrics.TryGetValue(um.name.ToLower(), out retrievedMetric))
            {
                // Check to see if each of the metrics in the upsert are in the retireved metrics.
                // If not add them otheriwse update if they have changed.
                foreach (long key in um.metrics.Keys)
                {
                    float value;
                    if (!retrievedMetric.Values.TryGetValue(key, out value))
                    {
                        // The value is not in the dictionary so add it
                        retrievedMetric.Values.Add(key, um.metrics[key]);
                    }
                    else
                    {
                        // The value exists so only update the dictionary if the value has changed
                        if (value != um.metrics[key])
                        {
                            retrievedMetric.Values[key] = um.metrics[key];
                        }
                    }
                }
            }
            else
            {
                // Add the new metrics to the store
                var newMetric = new Metric(um.name.ToLower(), um.metrics);
                _metricStore.metrics.Add(um.name.ToLower(), newMetric);
            }

            _log.Debug("Metrics received. VMName={0} Date={1}", _metricStore.vmName, _metricStore.date);
            foreach (KeyValuePair<long, float> entry in um.metrics)
            {
                _log.Debug("Metric={0}. Time={1} Value={2}", um.name, DateTime.FromBinary(entry.Key), entry.Value);
            }
        }
    }
}
