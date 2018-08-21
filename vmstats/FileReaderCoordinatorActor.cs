using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace vmstats
{
    class FileReaderCoordinatorActor : ReceiveActor
    {
        private IActorRef _fileReaderActor;
        private IActorRef _metricAccumulatorDispatcherActor;
        private string vmNamePattern;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        protected override void PreStart()
        {
            _log.Info($"Creating file read actor pool with vmNamePattern={vmNamePattern}");
            _fileReaderActor = Context.ActorOf(Props.Create(() =>
            new FileReaderActor(vmNamePattern, _metricAccumulatorDispatcherActor))
                .WithRouter(new RoundRobinPool(10)).WithDispatcher("vmstats-default-dispatcher"));
        }

        public FileReaderCoordinatorActor(string vmNamePattern, IActorRef metricDispatcher)
        {

            this.vmNamePattern = vmNamePattern;
            _metricAccumulatorDispatcherActor = metricDispatcher;
            _log.Info($"Initializing with vmNamePattern={vmNamePattern}");
        }


        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                maxNrOfRetries: 5,
                withinTimeRange: TimeSpan.FromMinutes(1),
                localOnlyDecider: ex =>
                {
                    // TODO Figure out what supervisor strategy is needed and implement it. For now just log the error and move on
                    _log.Error($"Actor error detected. Error is {ex.Message}. Error occurred at {ex.StackTrace}");
                    switch (ex)
                    {
                        /*
                        case ArithmeticException ae:
                            return Directive.Resume;
                        case NullReferenceException nre:
                            return Directive.Restart;
                        case ArgumentException are:
                            return Directive.Stop;
                        */
                        default:
                            return Directive.Escalate;
                    }
                });
        }

    }

}
