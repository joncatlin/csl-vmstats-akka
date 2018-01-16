using System;
using System.IO;
using Akka.Actor;
using Akka.Routing;
using Akka.Event;

namespace vmstats
{
    class DirectoryWatcherActor : UntypedActor
    {
        #region Message classes
        public class InitializeCommand
        {
            public InitializeCommand(string name, string type)
            {
                dirName = name;
                fileType = type;
            }

            public string dirName { get; private set; }
            public string fileType { get; private set; }
        }


        public const string CheckDirCommand = "check directory";
        #endregion

        #region Local variables
        private string _fileType;
        private string _dirName;
        private readonly string vmNamePattern;
        #endregion

        private IActorRef _fileReaderActor;
        private IActorRef _metricAccumulatorDispatcherActor;
        private ILoggingAdapter _log;


        public DirectoryWatcherActor(string vmNamePattern, IActorRef metricDispatcher)
        {
            _log = Context.GetLogger();
            this.vmNamePattern = vmNamePattern;
            _metricAccumulatorDispatcherActor = metricDispatcher;
        }


        protected override void PreStart()
        {
            _log.Info("In PreStart() - creating the router for the FileReaderActors");

            _fileReaderActor = Context.ActorOf(Props.Create(() =>
                new FileReaderActor(vmNamePattern, _metricAccumulatorDispatcherActor))
                .WithRouter(new RoundRobinPool(10)));
        }


        protected override void OnReceive(object message)
        {
            if (message is InitializeCommand)
            {
                var msg = message as InitializeCommand;
                _fileType = msg.fileType;
                _dirName = msg.dirName;
                _log.Info("Initialize received with dirName={0}, fileType={1}", _dirName, _fileType);

            }
            else if (message is CheckDirCommand)
            {
                ProcessCheckDir();
            }
        }


        // Find any files in the directory
        private void ProcessCheckDir()
        {
            Boolean noFilesFound = true;

            _log.Info("Checking directory for new files");
            foreach (string file in Directory.EnumerateFiles(_dirName, "*.csv", SearchOption.TopDirectoryOnly))
            {
                noFilesFound = false;

                _log.Info("Found file {0}", file);

                // Start an actor in the pool to deal with the new file
                _fileReaderActor.Tell(new FileReaderActor.Process(file));
            }

            if (noFilesFound)
            {
                // Signal end of metrics to all metric accumulators
                _metricAccumulatorDispatcherActor.Tell(new Messages.NoMoreMetrics());
            }
        }


    }
}
