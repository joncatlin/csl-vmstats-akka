using System;
using System.Collections.Generic;
using vmstats_shared;

namespace vmstats
{
    public class CombineTransformActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "COM";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";
        public static readonly string TRANSFORM_COLLECTION_START = "(";
        public static readonly string TRANSFORM_COLLECTION_END = ")";
        public static readonly string TRANSFORM_COLLECTION_CONTATENATOR = "+";

        
        // Parameters for the transform
        // TODO plumb the DSL parser to create a parameter using the following name that contains the number of transforms in the combine
        public static readonly string TRANSFORM_PARAM_COUNT_NAME = "C";

        // The store to hold the transforms in pending receiving all of them to be be combined
        Dictionary<string, List<Messages.TransformSeries>> TransformSereiesHoldingStore = new Dictionary<string, List<Messages.TransformSeries>>();

        public CombineTransformActor()
        {
            Receive<Messages.TransformSeries>(msg => ProcessRequest(msg));
        }


        private void ProcessRequest(Messages.TransformSeries msg)
        {
            // Create a key from the vmName, groupId and the date as multiple combines will have the same group id and only
            // the groupId + vmDate + vmName are unique.
            string key = msg.VmName + "-" + msg.VmDate + "-" + msg.GroupID.ToString();

            _log.Debug($"Received transform series for combining. Key: {key}");

            // Check to see if there are any transforms already received and stored which have the same key
            if (TransformSereiesHoldingStore.ContainsKey(key))
            {
                _log.Debug($"Already have some transforms for key: {key}");

                // There are some transforms with the same id. Check to see if all of them have been received.
                var numExpected = Convert.ToInt32(msg.Transforms.Dequeue().Parameters[TRANSFORM_PARAM_COUNT_NAME]);
                var storedTransforms = TransformSereiesHoldingStore[key];

                if (storedTransforms.Count == numExpected - 1)
                {
                    _log.Debug($"All transforms now received for Key: {key}. Combining the metrics from each and then routing.");

                    // Enough transforms have been received so combine them
                    storedTransforms.Add(msg);
                    var metric = Combine(storedTransforms);

                    // Route the result of the combine transform
                    var series = new Messages.TransformSeries(metric, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
                    RouteTransform(series);
                }
                else
                {
                    _log.Debug($"Still waiting for some transforms to be received, storing received transforms with others. Key: {key}");
 
                    // Still waiting for some of the metrics in the combine to be received so store this one
                    storedTransforms.Add(msg);
                }
            }
            else
            {
                _log.Debug($"This is the first transform with Key: {key}. Storing it and awaiting the rest.");

                // There are no entries for this TransformSeries, this is the first one
                var list = new List<Messages.TransformSeries>();
                list.Add(msg);
                TransformSereiesHoldingStore.Add(key, list);
            }

        }


        private Metric Combine(List<Messages.TransformSeries> transforms)
        {
            var combinedValues = new SortedDictionary<long, float>();

            // Combine each metric with the others
            foreach (var transform in transforms)
            {
                foreach (KeyValuePair<long, float> entry in transform.Measurements.Values)
                {
                    if (combinedValues.ContainsKey(entry.Key))
                    {
                        // Value already exists so add this value to it and store
                        var val = combinedValues[entry.Key];
                        val += entry.Value;
                        combinedValues.Remove(entry.Key);
                        combinedValues.Add(entry.Key, val);
                    }
                    else
                    {
                        // No entry exists so just add this value
                        combinedValues.Add(entry.Key, entry.Value);
                    }
                }
            }

            // Create the new name for the metric by combining the names from each transform
            var sb = new System.Text.StringBuilder();
            sb.Append(TRANSFORM_COLLECTION_START);
            var first = true;
            foreach (var transform in transforms)
            {
                if (!first)
                {
                    sb.Append(TRANSFORM_COLLECTION_CONTATENATOR);
                }
                else
                {
                    first = false;
                }
                sb.Append(transform.Measurements.Name);
            }
            sb.Append(TRANSFORM_COLLECTION_END);
            sb.Append(TRANSFORM_NAME_CONCATENATOR);
            sb.Append(TRANSFORM_NAME);

            // Create the new Metric from the combined values and return it to the caller
            var metric = new Metric(sb.ToString(), combinedValues);
            return metric;
        }
    }
}





