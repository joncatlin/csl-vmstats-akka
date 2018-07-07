using System;
using System.IO;
using Akka.Actor;
using Akka.Routing;
using Akka.Event;
using System.Collections.Generic;

namespace vmstats
{
    class DirectoryWatcherActor : ReceiveActor
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


        public class CheckDirCommand { };
        #endregion

        #region Local variables
        private string _fileType;
        private string _dirName;
        private readonly string vmNamePattern;
        #endregion

        private IActorRef _fileReaderActor;
        private IActorRef _metricAccumulatorDispatcherActor;
        private ILoggingAdapter _log;
        private Dictionary<string, int> foundFiles = new Dictionary<string, int>();
        enum State { FoundOnce, FoundMultiple, Delete };


        public DirectoryWatcherActor(string vmNamePattern, IActorRef metricDispatcher)
        {
            _log = Context.GetLogger();
            this.vmNamePattern = vmNamePattern;
            _metricAccumulatorDispatcherActor = metricDispatcher;

            Receive<InitializeCommand>(msg => Initialize(msg));
            Receive<CheckDirCommand>(msg => ProcessCheckDir());
        }


        protected override void PreStart()
        {
            _log.Info("In PreStart() - creating the router for the FileReaderActors");

            _fileReaderActor = Context.ActorOf(Props.Create(() =>
                new FileReaderActor(vmNamePattern, _metricAccumulatorDispatcherActor))
                .WithRouter(new RoundRobinPool(10)));
        }


        private void Initialize(InitializeCommand msg)
        {
            _fileType = msg.fileType;
            _dirName = msg.dirName;
            _log.Info("Initialize received with dirName={0}, fileType={1}", _dirName, _fileType);
        }

        // Find any files in the directory
        private void ProcessCheckDir()
        {
            Boolean noFilesFound = true;

            // Reset the set of foundFiles so it is obvious which ones have gone
            var keyList = new List<string>(foundFiles.Keys);
            foreach (var key in keyList)
            {
                foundFiles[key] = (int)State.Delete;
            }

            _log.Info("Checking directory for new files");
            foreach (string file in Directory.EnumerateFiles(_dirName, "*.csv", SearchOption.TopDirectoryOnly))
            {
                noFilesFound = false;

                _log.Info("Found file {0}", file);

                // See if the file has already been found
                if (!foundFiles.ContainsKey(file))
                {
                    // Start an actor in the pool to deal with the new file
                    _fileReaderActor.Tell(new FileReaderActor.Process(file));

                    // Add the file to the set of found files
                    foundFiles.Add(file, (int)State.FoundOnce);
                }
                else
                {
                    foundFiles[file] = (int)State.FoundMultiple;
                }
            }

            // For each file in the set of foundFiles that has the state of Delete then remove the item
            // from the set
            keyList = new List<string>(foundFiles.Keys);
            foreach (var key in keyList)
            {
                if (foundFiles[key] == (int)State.Delete) foundFiles.Remove(key);
            }

            if (noFilesFound)
            {
                // Signal end of metrics to all metric accumulators
                _metricAccumulatorDispatcherActor.Tell(new Messages.NoMoreMetrics());
            }
        }


    }
}
