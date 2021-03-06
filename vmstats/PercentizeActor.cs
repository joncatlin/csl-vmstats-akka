﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;
using vmstats_shared;

namespace vmstats
{
    public class PercentizeActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "PCT";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public PercentizeActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Create an array to hold the transformed values
            float[] valuesArray = new float[msg.Measurements.Values.Count];
            //            msg.Measurements.Values.Values.CopyTo(valuesArray, 0);

            // Find the max and min values in the data
            float max = float.MinValue;
            float min = float.MaxValue;

            foreach (var entry in msg.Measurements.Values)
            {
                if (entry.Value > max) max = entry.Value;
                if (entry.Value < min) min = entry.Value;
            }

            // Take the min value from the max as the min value is going to become zero after the
            // transform
            max -= min;

            // Make sure that the max is not zero to prevent divide by zero
            var newValues = new SortedDictionary<long, float>();
            if (max != 0.0F)
            {
                // Tansform the Metric data into percentages, where max is 100% and min is 0%
                foreach (var entry in msg.Measurements.Values)
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





