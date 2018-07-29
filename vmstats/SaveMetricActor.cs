using System;
using System.Collections.Generic;
using vmstats_shared;

namespace vmstats
{
    public class SaveMetricActor : BaseTransformActor
    {
        public static readonly string SAVE_NAME = "SAVE_NAME";
        public static readonly string SAVE_NAME_DEFAULT_VALUE ="TEMP";
        public static readonly string TRANSFORM_NAME = "SAV";
        public static readonly string TRANSFORM_NAME_CONCATENATOR = ":";

        public SaveMetricActor()
        {
            Receive<Messages.TransformSeries>(msg => CalculateTransformation(msg));
        }

        private void CalculateTransformation(Messages.TransformSeries msg)
        {
            // Get the transform from the series
            var transform = msg.Transforms.Dequeue();

            // Calculate the rolling average and use it as the base noise level
            string saveName = (transform.Parameters.ContainsKey(SAVE_NAME)) ? transform.Parameters[SAVE_NAME] :
                SAVE_NAME_DEFAULT_VALUE;

            // Construct the UPSERT msg so the values can be saved in the correct MetricStore
            var um = new MetricStoreActor.UpsertMetric(saveName, msg.Measurements.Values);

            // Find the correct MetricStore actor and send it the UPSERT msg
            var actorName = "/user/*/MetricStore:" + msg.VmName + ":" + msg.VmDate;
            var actor = Context.ActorSelection(actorName);
            actor.Tell(um);

            // Route the Metric unchanged to the next transform
            var series = new Messages.TransformSeries(msg.Measurements, msg.Transforms, msg.GroupID, msg.ConnectionId, msg.VmName, msg.VmDate);
            RouteTransform(series);
        }


    }
}





