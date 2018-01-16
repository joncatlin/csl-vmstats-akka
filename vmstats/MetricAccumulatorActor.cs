using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Akka.Event;

namespace vmstats
{
    public class MetricAccumulatorActor : ReceiveActor
    {
        #region Instance variables
        private readonly string vmName;
        private readonly string date;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);



        // TODO Chenage this value when the metric files have been corrected to contain the right number of items.
        // Currently they are missing every other hour
        static readonly int NUM_MEASUREMENTS = (int)720 - 20; // Assume a couple of metrics are missing, hence the minus 20.

        // Dictionary to hold all of the metrics in their raw state
        private SortedDictionary<long, float[]> metrics = new SortedDictionary<long, float[]>();

        // Ref to the actor that will store ant metrics accumulated by this actor
        IActorRef metricStoreActor;
        #endregion

        public MetricAccumulatorActor(string vmName, string date, IActorRef metricStoreActor)
        {
            // Initialize the patterns used to scan the file for metrics
            this.vmName = vmName;
            this.date = date;
            this.metricStoreActor = metricStoreActor;
            Receive<Messages.MetricsToBeProcessed>(msg => Accumulate(msg));
            Receive<Messages.NoMoreMetrics>(msg => NoMoreMetrics());
        }


        private void Accumulate(Messages.MetricsToBeProcessed msg)
        {
            _log.Debug("This accumulator is for vmName={0}, date={1}. Process data request for vmName={2}, date={3}, time={4}, elements={5}", 
                vmName, date, msg.vmName, msg.date, msg.time, msg.elements);

            // Save the information
            float[] value;
            if (!metrics.TryGetValue(msg.time, out value))
            {
                metrics.Add(msg.time, msg.elements);
            } else
            {
                metrics[msg.time] = msg.elements;
            }
        }


        private void NoMoreMetrics()
        {
            // Check to see if there are enough metrics to store
            if (metrics.Count >= NUM_MEASUREMENTS)
            {
                // There are enough so transform the metrics and then send them for storage
                SendMetrics();
            } else
            {
                _log.Info("This accumulator is for vmName={0}, date={1}. Process NoMoreMetrics. Not enough metrics accumulated, count={2}",
                    vmName, date, metrics.Count);
            }

        }

        private void SendMetrics()
        {
            _log.Info("This accumulator is for vmName={0}, date={1}. Sending metrics count={2}",
                vmName, date, metrics.Count);
            var um = new MetricStoreActor.UpsertMetric(vmName, metrics);
            metricStoreActor.Tell(um);

            // Tell all the metric accumulators dispatchers that this actor is finishing
            var selection = Context.ActorSelection("/*/MetricAccumulatorDispatcher*");
            selection.Tell(new Messages.Stopping());

            Context.Stop(Context.Self);

        }
    }
}
