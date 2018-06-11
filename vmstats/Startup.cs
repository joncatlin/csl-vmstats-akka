using System;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Akka.Event;

namespace vmstats
{
    public sealed class Startup
    {
        // This is a Singleton
        private static readonly Startup instance = new Startup();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Startup()
        {
        }

        private Startup()
        {

        }

        public static Startup Instance
        {
            get
            {
                return instance;
            }
        }

        // Names of all the environment variables needed by the application
        static readonly string ENV_DIRNAME = "DIR_NAME";
	    static readonly string ENV_FILETYPE = "FILE_TYPE";
	    static readonly string ENV_VMNAME_PATTERN = "VMNAME_PATTERN";

        // Local state initialized from envornment variables
        private string fileType = null;
        private string dirName = "dummyfilename";
        private string vmNamePattern = null;

        public ILoggingAdapter _log { get; set; }

        public void Start()
        {
            // Get the configuration of the akka system
            var config = ConfigurationFactory.ParseString(GetConfiguration());

            // Create the container for all the actors
            var vmstatsActorSystem = ActorSystem.Create("vmstats", config);
            _log = vmstatsActorSystem.Log;

            // Initialise state from environment variables
            GetEnvironmentVariables();

            // Create the metric dispatcher
            Props metricAccumulatorDispatcherProps = Props.Create(() => new MetricAccumulatorDispatcherActor());
            IActorRef metricAccumulatorDispatcherActor = vmstatsActorSystem.ActorOf(metricAccumulatorDispatcherProps,
                "metricAccumulatorDispatcherActor");

            // Create the actor that will watch the directory for new files being added
            Props directoryWatcherProps = Props.Create(() => new DirectoryWatcherActor(vmNamePattern, metricAccumulatorDispatcherActor));
            IActorRef directoryWatcherActor = vmstatsActorSystem.ActorOf(directoryWatcherProps,
                "directoryWatcherActor");

            // Initialize the actor and then get it to check the directory for files
            directoryWatcherActor.Tell(new DirectoryWatcherActor.InitializeCommand(dirName, fileType));

            // Schedule the DirectoryWatcher to check the directory
            vmstatsActorSystem.Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(30),
                          directoryWatcherActor, DirectoryWatcherActor.CheckDirCommand, ActorRefs.NoSender);

            // Wait until actor system terminated
//            vmstatsActorSystem.WhenTerminated.Wait();
        }

        private void GetEnvironmentVariables ()
        {
            dirName = GetEnvironmentVariable(ENV_DIRNAME);
            fileType = GetEnvironmentVariable(ENV_FILETYPE);
            vmNamePattern = GetEnvironmentVariable(ENV_VMNAME_PATTERN);
        }


        private string GetEnvironmentVariable (string envVarName)
        {
            string temp = Environment.GetEnvironmentVariable(envVarName);
            if (temp == null)
            {
                // Log an error and exit the program
                _log.Error($"ERROR: Missing environment variable named: {envVarName}");
                System.Environment.Exit(-1);
            }

            // Output the env variable's value
            _log.Info($"Environment variable initialized. Variable: {envVarName}. Value: {temp}");
            return temp;
        }


        private static string GetConfiguration()
        {
            string config = @"
                akka {  
                    stdout-loglevel = DEBUG
                    loglevel = DEBUG
#                    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                    log-config-on-start = on

#                    actor
#                    {
#                      debug
#                      {
#                        receive = on      # log any received message
#                        autoreceive = on  # log automatically received messages, e.g. PoisonPill
#                        lifecycle = on    # log actor lifecycle changes
#                        event-stream = on # log subscription changes for Akka.NET event stream
#                        unhandled = on    # log unhandled messages sent to actors
#                      }
#                    }
#                  }

                # Dispatcher for the Snapshot file store
 #               snapshot -dispatcher {
 #                   type = Dispatcher
 #                   throughput = 10000
 #               }


            ";

            return config;
        }
    }
}
