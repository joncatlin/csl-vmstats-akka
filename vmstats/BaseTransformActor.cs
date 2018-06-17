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
            var tname = series.Transforms.Peek().Name;

            // look up the actor with the name of the transform and send the transform series 
            // to it for the first step in the processing. Each transform actor in turn will then
            // use this method to route to the next transform in the series.
            Context.ActorSelection("**/Transforms:" + tname).Tell(series);
        }

    }
}





