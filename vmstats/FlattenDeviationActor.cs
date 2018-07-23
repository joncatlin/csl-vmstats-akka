using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;
using vmstats_shared;

namespace vmstats
{
    /// <summary>
    /// This actor truncates a series of values based on the deviation from the mode of the data.
    /// </summary>
    public class FlattenDeviationActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "FLD";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public static readonly string SD_TO_FLATTEN = "SD_TO_FLATTEN";
        public static readonly int SD_TO_FLATTEN_DEFAULT_VALUE = 1;

        public FlattenDeviationActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }


        /// <summary>
        /// Calculates the average given a series of measurements
        /// </summary>
        /// <param name="msg">A transform series that contains a series of measurements</param>
        /// <returns>The calaculated mean</returns>
        private float Mean(Messages.TransformSeries msg)
        {
            float sum = 0.0F;

            foreach (var entry in msg.Measurements.Values)
            {
                sum += entry.Value;
            }

            return sum / msg.Measurements.Values.Count;
        }


        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Get the value of the sd to flatten parameter if specified, otherwiuse use the default
            int sdToFlatten = (transform.Parameters.ContainsKey(SD_TO_FLATTEN)) ? Int32.Parse(transform.Parameters[SD_TO_FLATTEN]) :
                SD_TO_FLATTEN_DEFAULT_VALUE;

            // Get the mean of the values
            var mean = Mean(msg);

            // A new sorted dictionary to store the values in
            var newValues = new SortedDictionary<long, float>();

            // Calculate the standard devaition for the sample
            float sum = 0.0F;
            foreach (var entry in msg.Measurements.Values)
            {
                sum += (entry.Value - mean)* (entry.Value - mean);
            }
            float sd = (float)Math.Sqrt((double)(sum / msg.Measurements.Values.Count));

            // Flattern any points in the original measurement so that if a value is greater than the specified
            // number of standard deviations from the mean value its value is set to the mean plus the specified
            // number of standard deviations
            foreach (var entry in msg.Measurements.Values)
            {
                if (entry.Value > (mean + (sd * sdToFlatten)))
                {
                    newValues.Add(entry.Key, mean + (sd * sdToFlatten));
                }
                else
                {
                    newValues.Add(entry.Key, entry.Value);
                }
            }

            // Route the new Metric to the next transform
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);
            var series = new Messages.TransformSeries(metric, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }
    }
}





