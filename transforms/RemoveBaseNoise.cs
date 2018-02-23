using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using vmstats.Metric;

namespace transforms
{
    #region Message classes
    public class Transform
    {
        public Transform(Metric metr, Dictionary<string, string>paramaters)
        {
            Metric = metric;
            Parameters = paramaters;
        }

        public Transform(SortedDictionary<long, float> metrics)
        {
            Metric = metrics;
            Parameters = new Dictionary<string, string>();
        }

        public SortedDictionary<long, float> Metrics { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
    #endregion

    public class RemoveBaseNoise : ReceiveActor
    {
        public static readonly string ROLLING_AVG_LENGTH = "ROLLING_AVG_LENGTH";
        public static readonly int ROLLING_AVG_LENGTH_DEFAULT_VALUE = 10;

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public RemoveBaseNoise()
        {
            Receive<Transform>(msg => CalculateTransformation(msg));

        }

        private void CalculateTransformation(Transform msg)
        {
            // Calculate the rolling average and use it as the base noise level
            int rollingAvgLength = (msg.Parameters.ContainsKey(ROLLING_AVG_LENGTH)) ? Int32.Parse(msg.Parameters[ROLLING_AVG_LENGTH]) : 
                ROLLING_AVG_LENGTH_DEFAULT_VALUE;
            int index = 0;
            float baseNoise = FindLowestRollingAvg(msg.Metrics, index, rollingAvgLength);

            // Subtract the base noise level from the values in the message
            Dictionary<long, float> newValues = new Dictionary<long, float>();
            foreach (KeyValuePair<long, float> entry in msg.Metrics)
            {
                newValues.Add(entry.Key, Math.Max(entry.Value - baseNoise,0));
            }
        }



        /**
         * This method calculates the smallest rolling avg over a series of datapoints for a given rolling average length
         * @return 
         */
        private float FindLowestRollingAvg(SortedDictionary<long, float> values, int index, int rollingAvgLength)
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

            _log.Debug("Rolling average: {0}, max: {1}, avg: {2}, rolling avg position in data: {3}", rollingAvg, max, avg, rollingAvgPosition );

            return rollingAvg;
        }



    }
}





