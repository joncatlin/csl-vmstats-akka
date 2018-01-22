using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;

namespace transforms
{
    #region Message classes
    public class Transform
    {
        public Transform(SortedDictionary<long, float> metrics)
        {
            this.metrics = new SortedDictionary<long, float>();
        }

        public SortedDictionary<long, float> metrics { get; set; }
    }
    #endregion

    public class RemoveBaseNoise : ReceiveActor
    {

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public RemoveBaseNoise()
        {
            Receive<Transform>(msg => CalculateTransformation(msg));

        }

        private void CalculateTransformation(Transform msg)
        {
            // Calculate the rolling average and use it as the base noise level
            // TODO determine where the index and rollingAvgLength will come from
            int rollingAvgLength = 10;
            int index = 0;
            float baseNoise = findLowestRollingAvg(msg.metrics, index, rollingAvgLength);

            // Subtract the base noise level from the values in the message
            Dictionary<long, float> newValues = new Dictionary<long, float>();
            foreach (KeyValuePair<long, float> entry in msg.metrics)
            {
                newValues.Add(entry.Key, entry.Value - baseNoise);
            }
        }



        /**
         * This method calculates the smallest rolling avg over a series of datapoints for a given rolling average length
         * @return 
         */
        private float findLowestRollingAvg(SortedDictionary<long, float> values, int index, int rollingAvgLength)
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









public class StatisticRemoveBase
{

    // Initialize the logger
    private final Logger LOGGER = Logger.getLogger(this.getClass().getName());


    private class Base
    {
        float rollingAvg;
        float max;
        float avg;
        int rollingAvgPosition;
    }


    public ArrayList<String> removeBase(ArrayList<String> metrics, float percentage, int sampleSize)
    {

        ArrayList<String> transformed = new ArrayList<String>(metrics.size());
        float temp;

        // First find the lowest rolling average for the data set
        Base result = findLowestRollingAvg(metrics, sampleSize);

        // Calculate a cutoff value to remove noise

        float cutOff = ((result.max - result.rollingAvg) / 100F * percentage) + result.rollingAvg;
        //		float cutOff = (result.max / 100F * percentage) + result.rollingAvg;

        // Zeroize any number less than the cutoff value to remove the background noise 
        for (int i = 0; i < metrics.size(); i++)
        {
            temp = Float.valueOf(metrics.get(i));
            temp = (temp < cutOff) ? 0 : temp - result.rollingAvg;
            transformed.add(String.valueOf(temp));
        }

        return transformed;

    }


    // TODO remove lowest rolling average of net min from the netavg data 


}

