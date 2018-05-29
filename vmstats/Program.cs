using System;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace vmstats
{
    class Program
    {
        // Names of all the environment variables needed by the application
	    static readonly string ENV_DIRNAME = "DIR_NAME";
	    static readonly string ENV_FILETYPE = "FILE_TYPE";
	    static readonly string ENV_VMNAME_PATTERN = "VMNAME_PATTERN";

	
        // Local state initialized from envornment variables
        static string fileType = null;
        static string dirName = "dummyfilename";
        static string vmNamePattern = null;

        static void Main(string[] args)
        {

            var configNL = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var consoleTarget = new ColoredConsoleTarget();
            configNL.AddTarget("console", consoleTarget);
            
            // Step 3. Set target properties 
//            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
  //          fileTarget.FileName = "${basedir}/file.txt";
 //           fileTarget.Layout = "${message}";

            // Step 4. Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            configNL.LoggingRules.Add(rule1);
            
            // Step 5. Activate the configuration
            LogManager.Configuration = configNL;
            









            /*
                        var logger = new LoggerConfiguration()
                            .WriteTo.ColoredConsole()
                            .WriteTo.RollingFile("logs\\vmstats-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Properties}{NewLine}{Exception}")
                            .MinimumLevel.Debug()
                            .CreateLogger();
                        Serilog.Log.Logger = logger;
            */

            // Get the configuration of the akka system
            var config = ConfigurationFactory.ParseString(GetConfiguration());

            // Initialise state from environment variables
            GetEnvironmentVariables();

            // Create the container for all the actors
            var vmstatsActorSystem = ActorSystem.Create("vmstats", config);

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
//            directoryWatcherActor.Tell(DirectoryWatcherActor.CheckDirCommand);

            // Schedule the DirectoryWatcher to check the directory
            vmstatsActorSystem.Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(30),
                          directoryWatcherActor, DirectoryWatcherActor.CheckDirCommand, ActorRefs.NoSender);


            // Wait until actor system terminated
            vmstatsActorSystem.WhenTerminated.Wait();
        }

        static void GetEnvironmentVariables ()
        {
            dirName = GetEnvironmentVariable(ENV_DIRNAME);
            fileType = GetEnvironmentVariable(ENV_FILETYPE);
            vmNamePattern = GetEnvironmentVariable(ENV_VMNAME_PATTERN);
        }


        static string GetEnvironmentVariable (string envVarName)
        {
            string temp = Environment.GetEnvironmentVariable(envVarName);
            if (temp == null)
            {
                // Log an error and exit the program
                System.Environment.Exit(-1);
            }

            return temp;
        }



















        private static string GetConfiguration()
        {
            string config = @"
                akka {  
                    stdout-loglevel = DEBUG
                    loglevel = DEBUG
                    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
                    log-config-on-start = on

                    actor
                    {
                      debug
                      {
                        receive = on      # log any received message
                        autoreceive = on  # log automatically received messages, e.g. PoisonPill
                        lifecycle = on    # log actor lifecycle changes
                        event-stream = on # log subscription changes for Akka.NET event stream
                        unhandled = on    # log unhandled messages sent to actors
                      }
                    }
                  }
                }

                # Dispatcher for the Snapshot file store
                snapshot -dispatcher {
                    type = Dispatcher
                    throughput = 10000
                }


            ";

            return config;
        }


    }
}
