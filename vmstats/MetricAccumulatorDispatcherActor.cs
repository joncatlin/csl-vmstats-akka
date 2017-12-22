using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;
using System.Collections;

namespace vmstats
{
    class MetricAccumulatorDispatcherActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> routingTable = new Dictionary<string, IActorRef>();
        private Dictionary<string, IActorRef> storeTable = new Dictionary<string, IActorRef>();

        public MetricAccumulatorDispatcherActor()
        {
            Receive<Messages.MetricsToBeProcessed>(msg => Dispatch(msg));
            Receive<Messages.NoMoreMetrics>(msg => NoMoreMetrics());
            Receive<Messages.Stopping>(msg => Stopping());
        }


        private void Dispatch (Messages.MetricsToBeProcessed msg)
        {
            // Lookup the Actor to handle this request
            string key = "MetricAccumulator:" + msg.vmName + ":" + msg.date;
            IActorRef actor;
            if (!routingTable.TryGetValue(key, out actor))
            {
                // Create the actor to store the Metrics once they have been accumulated
                actor = Context.ActorOf(Props.Create(() =>
                    new MetricStoreActor(msg.vmName, msg.date)), "MetricStore:" + key);
                storeTable.Add(key, actor);

                // Create the actor for the accumulator and store its reference
                actor = Context.ActorOf(Props.Create(() =>
                    new MetricAccumulatorActor(msg.vmName, msg.date, actor)), key);
                routingTable.Add(key, actor);


                // Dispatch the message
                actor.Tell(msg);
            }
            else
            {
                actor.Tell(msg);
            }
        }


        private void Stopping()
        {
            string name = Context.Sender.Path.Name;
            routingTable.Remove(name);
        }

        private void NoMoreMetrics()
        {
            // Signal to all the accumulators that there are no more metrics
            foreach (var kvp in routingTable)
            {
                IActorRef actor = kvp.Value;
                actor.Tell(new Messages.NoMoreMetrics());
            }
        }
    }

}
