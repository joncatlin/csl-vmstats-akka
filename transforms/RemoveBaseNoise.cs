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
        public Transform(SortedDictionary<long, float[]> metrics)
        {
            this.metrics = new SortedDictionary<long, float[]>();
        }

        public SortedDictionary<long, float[]> metrics { get; set; }
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

        }



        /**
         * This method calculates the smallest rolling avg over a series of datapoints for a given rolling average length
         * @return 
         */
        private void findLowestRollingAvg(SortedDictionary<long, float[]> metrics, int index, int rollingAvgLength)
        {
            foreach (KeyValuePair<long, float[]> entry in metrics)
            {

            }
            var metricsList = metrics.Values.CopyTo();
            for (int i=0; i < metrics.Count - rollingAvgLength; i++)
            {

                float sum = 0;
                for (int offset = 0; offset < rollingAvgLength; offset++)
                {
                    sum += metrics[]Float.valueOf(metrics.get(i + offset));
                }

            }



            float rollingAvg = float.MaxValue; // Large number so first average is always less
            int rollingAvgPosition = -1;
            float max = 0;
            float avg = 0;

            // Calculate the smallest rolling average, overall average and a max for the datapoints
            for (int i = 0; i < metrics.size() - rollingAvgLength; i++)
            {

                float sum = 0;
                for (int offset = 0; offset < rollingAvgLength; offset++)
                {
                    sum += Float.valueOf(metrics.get(i + offset));
                }

                // Initialize the position of the first rolling average
                rollingAvgPosition = rollingAvgLength - 1;

                // Calculate the new values for the comparison
                float newRollingAvg = sum / rollingAvgLength;
                float newMax = Float.valueOf(metrics.get(i + rollingAvgLength));

                // Store the new values if appropriate
                if (newRollingAvg < rollingAvg)
                {
                    rollingAvg = newRollingAvg;
                    rollingAvgPosition = i;
                }
                if (newMax > max) max = newMax;

                // Add to the overall average
                avg += Float.valueOf(metrics.get(i));
            }

            // The overall average is missing values so add them before calculating the avg
            for (int i = metrics.size() - rollingAvgLength; i < metrics.size(); i++)
                avg += Float.valueOf(metrics.get(i));
            avg = avg / metrics.size();

            Base result = new Base();
            result.rollingAvg = rollingAvg;
            result.max = max;
            result.avg = avg;
            result.rollingAvgPosition = rollingAvgPosition;

            LOGGER.log(Level.FINE, "Rolling average: {0}, max: {1}, avg: {2}, rolling avg position in data: {3}", new Object[] { result.rollingAvg, result.max, result.avg, result.rollingAvgPosition });

            return result;
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

