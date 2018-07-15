using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats_shared;

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
        static readonly int NUM_MEASUREMENTS = (int)1400 - 20; // Assume slightly over 1 hour of metrics are missing

        // Dictionary to hold all of the metrics in their raw state
        private Dictionary<string, SortedDictionary<long, float>> metrics = new Dictionary<string, SortedDictionary<long, float>>();

        // Ref to the actor that will store any metrics accumulated by this actor
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
            _log.Debug("This accumulator is for vmName={0}, date={1}. Process data request for vmName={2}, date={3}, time={4}, element={5}, element name={6}", 
                vmName, date, msg.vmName, msg.date, msg.time, msg.element, msg.elementName);

            // Get the list of elements accumulated for the element name
            if (!metrics.ContainsKey(msg.elementName))
            {
                // The metric is new so add it
                var newElements = new SortedDictionary<long, float>();
                newElements.Add(msg.time, msg.element);
                metrics.Add(msg.elementName, newElements);
            } else
            {
                // The metric exists so check if the element needs to be added or updated
                var elements = metrics[msg.elementName];
                float element;
                if (!metrics[msg.elementName].TryGetValue(msg.time, out element))
                {
                    elements.Add(msg.time, msg.element);
                }
                else
                {
                    elements[msg.time] = msg.element;
                }
            }
        }


        private void NoMoreMetrics()
        {
            // Check to see if there are enough metrics to store
            var i = metrics.Values.GetEnumerator();
            i.MoveNext();
            SortedDictionary<long, float> j = i.Current;
            int numOfElements = (j == null) ? 0 : j.Count;
            if (numOfElements >= NUM_MEASUREMENTS)
            {
                // There are enough so transform the metrics and then send them for storage
                _log.Info("This accumulator is for vmName={0}, date={1}. Sending metrics count={2}",
                    vmName, date, numOfElements);
                SendMetrics();
            } else
            {
                _log.Info("This accumulator is for vmName={0}, date={1}. Process NoMoreMetrics. Not enough metrics accumulated, count={2}",
                    vmName, date, numOfElements);
            }

        }

        private void SendMetrics()
        {

            foreach (KeyValuePair<string, SortedDictionary<long, float>> entry in metrics)
            {
                // do something with entry.Value or entry.Key
                var um = new MetricStoreActor.UpsertMetric(entry.Key, entry.Value);
                metricStoreActor.Tell(um);
            }


            // Tell all the metric accumulators dispatchers that this actor is finishing
            var selection = Context.ActorSelection("/*/MetricAccumulatorDispatcher*");
            selection.Tell(new Messages.Stopping());

            Context.Stop(Context.Self);

        }
    }
}
