using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;

namespace transforms
{
    public class RemoveBaseNoiseActor : BaseTransformActor
    {
        public static readonly string ROLLING_AVG_LENGTH = "ROLLING_AVG_LENGTH";
        public static readonly int ROLLING_AVG_LENGTH_DEFAULT_VALUE = 10;
        public static readonly string TRANSFORM_NAME = "RBN";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public RemoveBaseNoiseActor()
        {
            Receive<TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Pop();

            // Calculate the rolling average and use it as the base noise level
            int rollingAvgLength = (transform.Parameters.ContainsKey(ROLLING_AVG_LENGTH)) ? Int32.Parse(transform.Parameters[ROLLING_AVG_LENGTH]) : 
                ROLLING_AVG_LENGTH_DEFAULT_VALUE;
            int index = 0;
            float baseNoise = FindLowestRollingAvg(msg.Measurements.Values, index, rollingAvgLength);

            // Subtract the base noise level from the values in the message
            SortedDictionary<long, float> newValues = new SortedDictionary<long, float>();
            foreach (KeyValuePair<long, float> entry in msg.Measurements.Values)
            {
                newValues.Add(entry.Key, Math.Max(entry.Value - baseNoise,0));
            }

            // Route the new Metric to the next transform
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);
            var series = new TransformSeries(metric, msg.Transforms, msg.GroupID);
            RouteTransform(series);
        }



        /**
         * This method calculates the smallest rolling avg over a series of datapoints for a given rolling average length
         * @return 
         */
        private float FindLowestRollingAvg(SortedDictionary<long, float> values, int index, int rollingAvgLength)
        {
            float rollingAvg = 0.0F;

            // Get the values to be processed into an array
            float[] valuesArray = new float[values.Count];
            values.Values.CopyTo(valuesArray, 0);

            // Initialize variables
            float rollingSum = 0.0F;

            // Initialise the sum for the rollingAvgLength window using the first set of values from array 
            // and the initialize the start value for the rollingAvg
            for (int offset = 0; offset < rollingAvgLength; offset++)
            {
                rollingSum += valuesArray[offset];
            }
            rollingAvg = rollingSum / rollingAvgLength;
            int rollingAvgPosition = 0;

            // Loop through the remaining array and determine if there is a window of length rollingAvgtLength that has a better average
            for (int pos = rollingAvgLength; pos < valuesArray.Length; pos++)
            {
                // Remove the first item from the sum and add the next item in the values array
                rollingSum -= valuesArray[pos - rollingAvgLength];
                rollingSum += valuesArray[pos];

                // Calucluste the new average
                var newAvg = rollingSum / rollingAvgLength;

                // Save the new average if it is less than the current one
                if (newAvg < rollingAvg)
                {
                    rollingAvg = newAvg;
                    rollingAvgPosition = pos - rollingAvgLength + 1;
                }
            }

            _log.Debug("Rolling average: {0}, rolling avg position in data: {1}", rollingAvg, rollingAvgPosition);

            return rollingAvg;
        }


        private float FindLowestRollingAvgOLD(SortedDictionary<long, float> values, int index, int rollingAvgLength)
        {
            // Get the values to be processed into an array
            float[] valuesArray = new float[values.Count];
            values.Values.CopyTo(valuesArray, 0);

            // Initialize variables
            float rollingAvg = float.MaxValue; // Large number so first average is always less
            int rollingAvgPosition = -1;
            float max = 0;
            float avg = 0;

            // Calculate the smallest rolling average, overall average and a max for the datapoints
            for (int i = 0; i < valuesArray.Length - rollingAvgLength; i++)
            {
                float sum = 0;
                for (int offset = 0; offset < rollingAvgLength; offset++)
                {
                    sum += valuesArray[i + offset];
                }

                // Initialize the position of the first rolling average
                rollingAvgPosition = rollingAvgLength - 1;

                // Calculate the new values for the comparison
                float newRollingAvg = sum / rollingAvgLength;
                float newMax = valuesArray[i + rollingAvgLength];

                // Store the new values if appropriate
                if (newRollingAvg < rollingAvg)
                {
                    rollingAvg = newRollingAvg;
                    rollingAvgPosition = i;
                }
                if (newMax > max) max = newMax;

                // Add to the overall average
                avg += valuesArray[i];
            }

            // The overall average is missing values so add them before calculating the avg
            for (int i = valuesArray.Length - rollingAvgLength; i < valuesArray.Length; i++)
                avg += valuesArray[i];
            avg = avg / valuesArray.Length;

            _log.Debug("Rolling average: {0}, max: {1}, avg: {2}, rolling avg position in data: {3}", rollingAvg, max, avg, rollingAvgPosition);

            return rollingAvg;
        }


    }
}





