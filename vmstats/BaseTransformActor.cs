using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using vmstats;
using static vmstats.Messages;

namespace vmstats
{
    public class BaseTransformActor : ReceiveActor
    {
        protected readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public static void RouteTransform(TransformSeries series)
        {
            // Get the name of the first transform in the series
            string path = null;
            if (series.Transforms.Count > 0)
            {
                // Get the name of the next transform in the queue
                var tname = series.Transforms.Peek().Name;
                path = "/user/Transforms-" + tname.ToUpper();
            } else
            {
                // The queue is empty so all transforms have beenc completed so route back to the MetricStoreManager
                path = "/user/" + MetricStoreManagerActor.ACTOR_NAME;
            }

            // Look up the actor with the name of the transform and send the transform series 
            // to it for the first step in the processing. Each transform actor in turn will then
            // use this method to route to the next transform in the series.
            var ctx = Context.ActorSelection(path);
            ctx.Tell(series);


            //"akka://vmstats/user/Transforms-RBN"
        }

    }
}





