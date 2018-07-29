using System;
using System.Collections.Generic;
using vmstats_shared;

namespace vmstats
{
    public class ViewMetricActor : BaseTransformActor
    {
        public static readonly string TRANSFORM_NAME = "VIW";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public ViewMetricActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Route the current Metric to the final step so that the results are returned to the user
            var series = new Messages.TransformSeries(msg.Measurements, new Queue<Messages.Transform>(), msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);

            // Route the Metric unchanged to the next transform
            series = new Messages.TransformSeries(msg.Measurements, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }


    }
}





