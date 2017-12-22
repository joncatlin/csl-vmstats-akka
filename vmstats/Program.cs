using System;
using Serilog;
using Akka.Actor;
using Akka.Logger.Serilog;
using Akka.Configuration;
using System.IO;

namespace vmstats
{
    class Program
    {
        // Names of all the environment variables needed by the application
        static readonly string ENV_COUCHBASE_NODES = "COUCHBASE_NODES";
	    static readonly string ENV_DIRNAME = "DIR_NAME";
	    static readonly string ENV_FILETYPE = "FILE_TYPE";
	    static readonly string ENV_VMNAME_PATTERN = "VMNAME_PATTERN";

	
        // Local state initialized from envornment variables
	    static string couchbaseNodes = null;
        static string fileType = null;
        static string statsdServer = null;
        static string dirName = "dummyfilename";
        static string vmNamePattern = null;

        static void Main(string[] args)
        {
            // Load the configration from the resource.conf file
            //string contents = File.ReadAllText("resource.conf");
            //var config = ConfigurationFactory.ParseString(contents);

            var config = ConfigurationFactory.ParseString(@"log-config-on-start = on
	stdout-loglevel = INFO
	loglevel=INFO,  
	loggers=[""Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog""]}");



            /* COULDNOT GET THIS TO WORK
            log = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Information()
                .CreateLogger();
            Serilog.Log.Logger = log;
            //var system = ActorSystem.Create("my-test-system", "akka { loglevel=INFO,  loggers=[\"Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog\"]}");
            */

            // Initialise the logging framwork
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole()
                .WriteTo.RollingFile("logs\\vmstats-{Date}.txt")
                .CreateLogger();

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
            Log.Information("Actor System Waiting");
            vmstatsActorSystem.WhenTerminated.Wait();
        }


        static void GetEnvironmentVariables ()
        {
            couchbaseNodes = GetEnvironmentVariable(ENV_COUCHBASE_NODES);
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
                Log.Error("No Environment variable found for {0}. Exiting application.", envVarName);
                System.Environment.Exit(-1);
            }

            Log.Information("Environment variable {0} = {1}", envVarName, temp);
            return temp;
        }
    }
}
