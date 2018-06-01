using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;

namespace transforms
{
    public class CombineActor : ReceiveActor
    {
        public static readonly string TRANSFORM_NAME = "COM";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";
        public static readonly string TRANSFORM_COLLECTION_START = "(";
        public static readonly string TRANSFORM_COLLECTION_END = ")";
        public static readonly string TRANSFORM_COLLECTION_CONTATENATOR = "+";

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public CombineActor()
        {
            Receive<Combine>(msg => Combine(msg));
        }

        private void Combine(Combine msg)
        {
            // Get a list of the keys from the first metric. Use this list of keys to extract
            // the values out of each metric to be combined.
            // NOTE: This means that the new Metric created will only have keys from the first Metric and
            // if there are additional keys in the other Metric's then they will be lost.
            var combinedValues = new SortedDictionary<long, float>();
            foreach (KeyValuePair<long, float> entry in msg.Metrics[0].Values)
            {
                float newValue = 0.0F;
                foreach(Metric item in msg.Metrics)
                {
                    float temp = 0.0F;
                    item.Values.TryGetValue(entry.Key, out temp);
                    newValue += temp;
                }
                combinedValues.Add(entry.Key, newValue);
            }

            // Create the name of the new Metric by combining the names for each one in the list
            var sb = new System.Text.StringBuilder();
            sb.Append(TRANSFORM_COLLECTION_START);
            var first = true;
            foreach (Metric item in msg.Metrics)
            {
                if (!first)
                {
                    sb.Append(TRANSFORM_COLLECTION_CONTATENATOR);
                }
                else
                {
                    first = false;
                }
                sb.Append(item.Name);
            }
            sb.Append(TRANSFORM_COLLECTION_END);
            sb.Append(TRANSFORM_NAME_CONCATENATOR);
            sb.Append(TRANSFORM_NAME);

            // Return the results to the caller
            var result = new Result(new Metric(sb.ToString(), combinedValues));
            Sender.Tell(result);
        }
    }
}





