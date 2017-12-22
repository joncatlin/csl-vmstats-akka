using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;

namespace vmstats
{
    class FileReaderCoordinatorActor : ReceiveActor
    {
        private IActorRef _fileReaderActor;
        private IActorRef _metricAccumulatorDispatcherActor;
        private string vmNamePattern;

        protected override void PreStart()
        {
            _fileReaderActor = Context.ActorOf(Props.Create(() =>
                new FileReaderActor(vmNamePattern, _metricAccumulatorDispatcherActor))
                .WithRouter(new RoundRobinPool(10)));
        }

        public FileReaderCoordinatorActor(string vmNamePattern, IActorRef metricDispatcher)
        {
            this.vmNamePattern = vmNamePattern;
            _metricAccumulatorDispatcherActor = metricDispatcher;
        }


    }

}
