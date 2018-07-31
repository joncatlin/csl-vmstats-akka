using System;
using System.Collections.Generic;
using vmstats_shared;

namespace vmstats
{
    public class CompactTimeActor : BaseTransformActor
    {
        public static readonly string TIME_PERIOD = "TIME_PERIOD";  // In minutes
        public static readonly int TIME_PERIOD_DEFAULT_VALUE = 15;
        public static readonly string TRANSFORM_NAME = "CMP";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        class TempValue 
        {
            public TempValue(float value)
            {
                Value = value;
                Count = 1;
            }
            public float Value { get; set; }
            public int Count { get; set; }      // To calculate the average
        }

        public CompactTimeActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Obtain any changes in the default settings
            int timePeriod = (transform.Parameters.ContainsKey(TIME_PERIOD)) ? Int32.Parse(transform.Parameters[TIME_PERIOD]) :
                TIME_PERIOD_DEFAULT_VALUE;

            // Create the storage for the new values and the intermediate values usedin the calculation
            var newValues = new SortedDictionary<long, float>();
            var tempValues = new SortedDictionary<long, TempValue>();

            // Calculate the start of the day in ticks
            var startPeriodTicks = (Convert.ToDateTime(msg.VmDate)).Ticks;

            // Calculate the time period to compress to, in ticks
            var timePeriodTicks = TimeSpan.TicksPerMinute * timePeriod;

            // Foreach value in the metric get the time period it is for and add its value to the correct compressed time period
            foreach (var entry in msg.Measurements.Values)
            {
                // Determine the compressed time
                var compressedTimeTicks = (((entry.Key - startPeriodTicks) / timePeriodTicks) * timePeriodTicks) + startPeriodTicks;

                TempValue tempValue;
                if (tempValues.TryGetValue(compressedTimeTicks, out tempValue))
                {
                    // Compressed time period already exists so add this value
                    tempValue.Value += entry.Value;
                    tempValue.Count++;
                }
                else
                {
                    // Period does not exist so add it
                    tempValues.Add(compressedTimeTicks, new TempValue(entry.Value));
                }
            }

            // Create the Metric values by averaging the TempValues collected
            foreach (var entry in tempValues)
            {
                newValues.Add(entry.Key, entry.Value.Value / entry.Value.Count);
            }
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);

            // Route the new Metric to the next transform
            var series = new Messages.TransformSeries(metric, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }
    }
}





