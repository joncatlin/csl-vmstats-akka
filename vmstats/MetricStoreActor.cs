using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Persistence;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Akka.Event;

namespace vmstats
{
    public class MetricStore
    {
        public SortedDictionary<long, float[]> metrics { get; set; }
        public string vmName { get; set; }
        public string date { get; set; }

        public MetricStore (string vmName, string date)
        {
            this.vmName = vmName;
            this.date = date;
            this.metrics = new SortedDictionary<long, float[]>();
        }
    }

    public class MetricStoreActor : ReceivePersistentActor
    {
        #region Messages
        public class UpsertMetric
        {
            public UpsertMetric(string name, SortedDictionary<long, float[]> metrics)
            {
                this.name = name;
                this.metrics = metrics;
            }

            public string name { get; set; }
            public SortedDictionary<long, float[]> metrics { get; set; }
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
                ProcessUpsertMetric(um);
                SaveSnapshot(_metricStore);
            }));
            Command<SaveSnapshotSuccess>(success => {
                // soft-delete the journal up until the sequence # at
                // which the snapshot was taken
                DeleteMessages(success.Metadata.SequenceNr);
            });
            Command<SaveSnapshotFailure>(failure => {
                // handle snapshot save failure...
            });
        }


        private void ProcessUpsertMetric(UpsertMetric um)
        {
            // Check to see if each of the metrics in the upsert are in the retireved metrics.
            // If not add them otheriwse update if they have changed.
            foreach (long key in um.metrics.Keys)
            {
                float[] value;
                if (!_metricStore.metrics.TryGetValue(key, out value)) {
                    // The value is not in the dictionary so add it
                    _metricStore.metrics.Add(key, um.metrics[key]);
                } else
                {
                    // The value exists so only update the dictionary if the value has changed
                    if (value != um.metrics[key])
                    {
                        _metricStore.metrics[key] = um.metrics[key];
                    }
                }
            }
        }
    }
}
