using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;

namespace transforms
{
    public class RemoveSpikeActor : BaseTransformActor
    {
        public static readonly string SPIKE_WINDOW_LENGTH = "SPIKE_WINDOW_LENGTH";
        public static readonly int SPIKE_WINDOW_LENGTH_DEFAULT_VALUE = 1;
        public static readonly string BASE_WINDOW_LENGTH = "BASE_WINDOW_LENGTH";
        public static readonly int BASE_WINDOW_LENGTH_DEFAULT_VALUE = 1;
        public static readonly string BASE_VALUE = "BASE_VALUE";
        public static readonly int BASE_VALUE_DEFAULT_VALUE = 0;
        public static readonly string TRANSFORM_NAME = "RSP";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public RemoveSpikeActor()
        {
            Receive<TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Obtain any changes in the default settings
            int spikeWindowLength = (transform.Parameters.ContainsKey(SPIKE_WINDOW_LENGTH)) ? Int32.Parse(transform.Parameters[SPIKE_WINDOW_LENGTH]) :
                SPIKE_WINDOW_LENGTH_DEFAULT_VALUE;
            int baseWindowLength = (transform.Parameters.ContainsKey(BASE_WINDOW_LENGTH)) ? Int32.Parse(transform.Parameters[BASE_WINDOW_LENGTH]) :
                BASE_WINDOW_LENGTH_DEFAULT_VALUE;
            int baseValue = (transform.Parameters.ContainsKey(BASE_VALUE)) ? Int32.Parse(transform.Parameters[BASE_VALUE]) :
                BASE_VALUE_DEFAULT_VALUE;

            // Scan the values for spikes that match the windows size. A spike is determined by a series of base values followed by a 
            // series of values over base followed by a return to a series of values at base. E.g. 0,0,0,5,7,0,0,0. The spike would be 5 & 7.
            // Get the values to be processed into an array
            float[] valuesArray = new float[msg.Measurements.Values.Count];
            msg.Measurements.Values.Values.CopyTo(valuesArray, 0);

            // TODO complete this code, create test for this class, determine all the other transformation classes needed to analyze the results
            for (int index=0; index <= valuesArray.Length - spikeWindowLength - (baseWindowLength * 2); /* No auto increment */)
            {

                // Determine if the current location is the start of a base window
                bool inBaseWindow = true;
                for (int startBaseIndex = index; startBaseIndex < index + baseWindowLength; startBaseIndex++)
                {
                    // If the value at the current location in the values array is not a base value then stop the search and move on
                    if (valuesArray[startBaseIndex] > baseValue)
                    {
                        inBaseWindow = false;
                        break;
                    }
                }

                // Finish the search if the start base window condition is not met
                if (!inBaseWindow)
                {
                    index++;
                    continue;
                }

                // Determine if the current location has a window of base values at the end
                inBaseWindow = true;
                for (int endBaseIndex = index + baseWindowLength + spikeWindowLength; 
                    endBaseIndex < index + (baseWindowLength * 2) + spikeWindowLength; endBaseIndex++)
                {
                    // If the value at the current location in the values array is not a base value then stop the search and move on
                    if (valuesArray[endBaseIndex] > baseValue)
                    {
                        inBaseWindow = false;
                        break;
                    }
                }

                // Finish the search if the end base window condition is not met
                if (!inBaseWindow) {
                    index++;
                    continue;
                }

                // Since the start of the range and the end of the range contain windows where the values are equal to or less that the supplied base value
                // then the conditions for a spike have been met.
                // The conditions for a spkie have been detected so set the spkie window of values to the base value
                for (int spikeBaseIndex = index + baseWindowLength;
                    spikeBaseIndex < index + baseWindowLength + spikeWindowLength; spikeBaseIndex++)
                {
                    valuesArray[spikeBaseIndex] = baseValue;
                }

                // A spike was found so advance the position in the array by the start base window plus the spkie window
                index = index + baseWindowLength + spikeWindowLength;
            }

            // Created a new Metric from the keys of the one sent in the message and the new values created by removing spikes
            int count = 0;
            SortedDictionary<long, float> newValues = new SortedDictionary<long, float>();
            foreach (KeyValuePair<long, float> entry in msg.Measurements.Values)
            {
                newValues.Add(entry.Key, valuesArray[count++]);
            }
            var metric = new Metric(msg.Measurements.Name + TRANSFORM_NAME_CONCATENATOR + TRANSFORM_NAME, newValues);

            // Route the new Metric to the next transform
            var series = new TransformSeries(metric, msg.Transforms, msg.GroupID);
            RouteTransform(series);
        }
    }
}





