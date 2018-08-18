using System;
using Akka.Actor;
using System.IO;
using System.Text.RegularExpressions;
using Akka.Event;
using vmstats_shared;
using Akka.Routing;

namespace vmstats
{
    public class StartupActor : ReceiveActor
    {
        #region Messages
        public class Startup
        {
        }
        #endregion

        #region Class variables
        public static readonly string ACTOR_NAME = "STARTUP_ACTOR";

        // Names of all the environment variables needed by the application
        static readonly string ENV_DIRNAME = "DIR_NAME";
        static readonly string ENV_FILETYPE = "FILE_TYPE";
        static readonly string ENV_VMNAME_PATTERN = "VMNAME_PATTERN";
        static readonly string ENV_SNAPSHOT_PATH = "SNAPSHOT_PATH";
        static readonly string ENV_VMSTATSGUI_WEBSERVER_URL = "VMSTATSGUI_WEBSERVER_URL";
        #endregion

        #region Instance variables
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        // Local state initialized from envornment variables
        private string fileType = null;
        private string dirName = "dummyfilename";
        private string vmNamePattern = null;
        private string snapshotPath = null;
        private string guiWebserUrl = null;
        #endregion

        public StartupActor()
        {
            Receive<Startup>(msg => ProcessStartup(msg));
        }


        private void ProcessStartup(Startup msg)
        {
            _log.Info("Starting up all actors");
            GetEnvironmentVariables();

            /*
             * Create all of the transform actors so they are ready when needed. Each transform is a pool
             * of actors.
             */
            Props rbnProps = Props.Create(() => new RemoveBaseNoiseActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef rbn = Context.ActorOf(rbnProps, "Transforms-" + RemoveBaseNoiseActor.TRANSFORM_NAME.ToUpper());
            var rbnName = rbn.Path.ToString();

            // Create the RemoveSpikeNoise actor pool
            Props rspProps = Props.Create(() => new RemoveSpikeActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef rsp = Context.ActorOf(rspProps, "Transforms-" + RemoveSpikeActor.TRANSFORM_NAME.ToUpper());

            // Create the PercentizeActor actor pool
            Props pctProps = Props.Create(() => new PercentizeActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef pct = Context.ActorOf(pctProps, "Transforms-" + PercentizeActor.TRANSFORM_NAME.ToUpper());

            // Create the CombineTransformActor actor pool
            Props comProps = Props.Create(() => new CombineTransformActor()).WithDispatcher("my-dispatcher").WithRouter(new ConsistentHashingPool(5));
            IActorRef com = Context.ActorOf(comProps, "Transforms-" + CombineTransformActor.TRANSFORM_NAME.ToUpper());

            // Create the RemovePercentileActor actor pool
            Props rptProps = Props.Create(() => new RemovePercentileActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef rpt = Context.ActorOf(rptProps, "Transforms-" + RemovePercentileActor.TRANSFORM_NAME.ToUpper());

            // Create the RemovePercentileActor actor pool
            Props fldProps = Props.Create(() => new FlattenDeviationActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef fld = Context.ActorOf(fldProps, "Transforms-" + FlattenDeviationActor.TRANSFORM_NAME.ToUpper());

            // Create the RemoveLowValuesActor actor pool
            Props rlvProps = Props.Create(() => new RemoveLowValuesActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef rlv = Context.ActorOf(rlvProps, "Transforms-" + RemoveLowValuesActor.TRANSFORM_NAME.ToUpper());

            // Create the SaveMetricActor actor pool
            Props savProps = Props.Create(() => new SaveMetricActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef sav = Context.ActorOf(savProps, "Transforms-" + SaveMetricActor.TRANSFORM_NAME.ToUpper());

            // Create the ViewMetricActor actor pool
            Props viwProps = Props.Create(() => new ViewMetricActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef viw = Context.ActorOf(viwProps, "Transforms-" + ViewMetricActor.TRANSFORM_NAME.ToUpper());

            // Create the CompactTimeActor actor pool
            Props cmpProps = Props.Create(() => new CompactTimeActor()).WithDispatcher("my-dispatcher").WithRouter(new RoundRobinPool(5));
            IActorRef cmp = Context.ActorOf(cmpProps, "Transforms-" + CompactTimeActor.TRANSFORM_NAME.ToUpper());

            // Create the metric dispatcher
            Props metricAccumulatorDispatcherProps = Props.Create(() => new MetricAccumulatorDispatcherActor(snapshotPath, guiWebserUrl)).WithDispatcher("my-dispatcher");
            IActorRef metricAccumulatorDispatcherActor = Context.ActorOf(metricAccumulatorDispatcherProps,
                MetricAccumulatorDispatcherActor.ACTOR_NAME);
            _log.Debug("Creating the metricAccumulatorDispatcherActor");

            // Create the actor that will watch the directory for new files being added
            Props directoryWatcherProps = Props.Create(() => new DirectoryWatcherActor(vmNamePattern, metricAccumulatorDispatcherActor)).WithDispatcher("my-dispatcher");
            IActorRef directoryWatcherActor = Context.ActorOf(directoryWatcherProps,
                "directoryWatcherActor");
            _log.Debug("Creating the directoryWatcherActor");

            // Initialize the actor and then get it to check the directory for files
            directoryWatcherActor.Tell(new DirectoryWatcherActor.InitializeCommand(dirName, fileType));
            _log.Debug("Send DirectoryWatcherActor.InitializeCommand(dirName, fileType) message to directoryWatcherActor");

            // Schedule the DirectoryWatcher to check the directory
            Context.System.Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(30),
                          directoryWatcherActor, new DirectoryWatcherActor.CheckDirCommand(), ActorRefs.NoSender);
            _log.Debug("Scheduling the directoryWatcherActor with CheckDirCommand");

        }


        private void GetEnvironmentVariables()
        {
            dirName = GetEnvironmentVariable(ENV_DIRNAME);
            fileType = GetEnvironmentVariable(ENV_FILETYPE);
            vmNamePattern = GetEnvironmentVariable(ENV_VMNAME_PATTERN);
            snapshotPath = GetEnvironmentVariable(ENV_SNAPSHOT_PATH);
            guiWebserUrl = GetEnvironmentVariable(ENV_VMSTATSGUI_WEBSERVER_URL);
        }


        private string GetEnvironmentVariable(string envVarName)
        {
            string temp = Environment.GetEnvironmentVariable(envVarName);
            if (temp == null)
            {
                // Log an error and exit the program
                Console.WriteLine($"ERROR: Missing environment variable named: {envVarName}");
                System.Environment.Exit(-1);
            }

            // Output the env variable's value
            Console.WriteLine($"Environment variable initialized. Variable: {envVarName}. Value: {temp}");
            return temp;
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
