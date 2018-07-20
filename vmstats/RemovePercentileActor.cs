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

            // Create an array to hold the transformed values
            float[] valuesArray = new float[msg.Measurements.Values.Count];

            // Get the value of the percentile to remove
            int percentileToRemove = (transform.Parameters.ContainsKey(PERCENTILE_TO_REMOVE)) ? Int32.Parse(transform.Parameters[PERCENTILE_TO_REMOVE]) :
                PERCENTILE_TO_REMOVE_DEFAULT_VALUE;

            // Re-sort the values based on value and then time
            var valueOrder = new SortedDictionary<float, long>();
            foreach (var entry in msg.Measurements.Values)
            {
                // Combine the value and the time so they are sorted in value first then time order
                string newVal = entry.Value * 100000;
                newVal += "." + entry.Key;
                double sortValue = Convert.ToDouble(newVal);

                valueOrder.Add(sortValue, entry.Key);
            }

            // Sort the dictionary in ascensing order

            // Set the values that are in percentile covered by the percentileToRemove variable

            valueOrder.

            // Find the max and min values in the data
            float max = float.MinValue;
            float min = float.MaxValue;
            for (int index = 0; index < msg.Measurements.Values.Count; index++)
            {
                if (msg.Measurements.Values[index] > max)
                {
                    max = msg.Measurements.Values[index];
                }
                else if (msg.Measurements.Values[index] < min)
                {
                    min = msg.Measurements.Values[index];
                }
            }

            // Take the min value from the max as the min value is going to become zero after the
            // transform
            max -= min;

            // TODO Make sure that the max is not zero to prevent divide by zero
            SortedDictionary<long, float> newValues = new SortedDictionary<long, float>();
            if (max != 0.0F)
            {
                // Tansform the Metric data into percentages, where max is 100% and min is 0%
                foreach (KeyValuePair<long, float> entry in msg.Measurements.Values)
                {
                    if (entry.Value != 0.0F)
                    {
                        newValues.Add(entry.Key, (entry.Value - min) / max * 100.0F);
                    }
                    else
                    {
                        newValues.Add(entry.Key, 0.0F);
                    }
                }
            }
            else
            {
                // Copy over the keys and set all the values to zero
                foreach (KeyValuePair<long, float> entry in msg.Measurements.Values)
                {
                    newValues.Add(entry.Key, 0.0F);
                }
            }


            // Route the new Metric to the next transform
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);
            var series = new Messages.TransformSeries(metric, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }
    }
}





