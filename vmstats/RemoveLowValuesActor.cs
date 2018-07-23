using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;
using vmstats_shared;

namespace vmstats
{
    public class RemoveLowValuesActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "RLV";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public static readonly string VALUES_TO_REMOVE = "VALUES_TO_REMOVE";
        public static readonly int VALUES_TO_REMOVE_DEFAULT_VALUE = 1;

        public RemoveLowValuesActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Get the value of the percentile to remove
            int valuesToRemove = (transform.Parameters.ContainsKey(VALUES_TO_REMOVE)) ? Int32.Parse(transform.Parameters[VALUES_TO_REMOVE]) :
                VALUES_TO_REMOVE_DEFAULT_VALUE;

            // Re-sort the values based on value and then time, ignoring values that are zero
            var newValues = new SortedDictionary<long, float>();
            foreach (var entry in msg.Measurements.Values)
            {
                // Skip values that are zero
                if (entry.Value <= valuesToRemove)
                {
                    newValues.Add(entry.Key, 0.0F);
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





