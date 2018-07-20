using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;
using vmstats_shared;

namespace vmstats
{
    public class RemovePercentileActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "RPT";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public static readonly string PERCENTILE_TO_REMOVE = "PERCENTILE_TO_REMOVE";
        public static readonly int PERCENTILE_TO_REMOVE_DEFAULT_VALUE = 1;

        public RemovePercentileActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Get the value of the percentile to remove
            int percentileToRemove = (transform.Parameters.ContainsKey(PERCENTILE_TO_REMOVE)) ? Int32.Parse(transform.Parameters[PERCENTILE_TO_REMOVE]) :
                PERCENTILE_TO_REMOVE_DEFAULT_VALUE;

            // Re-sort the values based on value and then time, ignoring values that are zero
            var valueOrder = new SortedDictionary<double, long>();
            foreach (var entry in msg.Measurements.Values)
            {
                // Skip values that are zero
                if (entry.Value > 0.0F)
                {

                    // Combine the value and the time so they are sorted in value first then time order
                    string newVal = Convert.ToString((long)(entry.Value * 100000)); // Hopefully no value is less than 100000
                    newVal += "." + entry.Key;
                    double sortValue = Convert.ToDouble(newVal);

                    valueOrder.Add(sortValue, entry.Key);
                }
            }

            // Set the values that are in percentile covered by the percentileToRemove variable
            int index = 0;
            int count = valueOrder.Count / 100 * percentileToRemove;
            foreach (var entry in valueOrder)
            {
                if (index < count)
                {
                    // Remove the value because it is in the percentile
                    msg.Measurements.Values[entry.Value] = 0;
                    index++;
                }
                else
                {
                    // There are no more values in the percentile so stop processing
                    break;
                }
            }

            // Create a clone of the updated measurements
            var newValues = new SortedDictionary<long, float>(msg.Measurements.Values);

            // Route the new Metric to the next transform
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);
            var series = new Messages.TransformSeries(metric, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }
    }
}





