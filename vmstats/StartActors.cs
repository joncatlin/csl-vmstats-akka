using System;
using Akka.Actor;
using Akka.Configuration;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Akka.Event;
using Akka.Routing;

namespace vmstats
{
    public sealed class StartActors
    {
        // This is a Singleton
        private static StartActors instance = null;
        private static readonly object padlock = new object();

        // Instance variables
        static readonly string ENV_CONFIG_FILE = "CONFIG_FILE";
        private string configFile = null;

        StartActors()
        {
        }

        public static StartActors Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new StartActors();
                        instance.Initialize();
                    }
                    return instance;
                }
            }
        }

        public ActorSystem vmstatsActorSystem;

        public ILoggingAdapter _log { get; set; }

        private void Initialize()
        {
            GetEnvironmentVariables();

            // Get the configuration of the akka system
            if (!File.Exists(configFile))
            {
                Console.WriteLine($"ERROR: In StartActors.Initialize(). Configuration file does not exist. configPath={configFile}");
                System.Environment.Exit(-1);
            }

            string textConfig = File.ReadAllText(configFile);

//            textConfig = textConfig.Replace("$$PATH_REPLACE$$", snapshotPath);

            // Get the akka configuration
            var config = ConfigurationFactory.ParseString(textConfig);

            // Create the container for all the actors
            vmstatsActorSystem = ActorSystem.Create("vmstats", config);
            _log = vmstatsActorSystem.Log;

            Props startupProps = Props.Create(() => new StartupActor()).WithDispatcher("my-dispatcher");
            IActorRef startupActor = vmstatsActorSystem.ActorOf(startupProps, StartupActor.ACTOR_NAME);
            startupActor.Tell(new StartupActor.Startup());
            _log.Debug("Creating the startup actor to start all other actors");

            /*
             * Create all of the transform actors so they are ready when needed. Each transform is a pool
             * of actors.
             */
            /*
            Props rbnProps = Props.Create(() => new RemoveBaseNoiseActor()).WithRouter(new RoundRobinPool(5));
            IActorRef rbn = vmstatsActorSystem.ActorOf(rbnProps, "Transforms-" + RemoveBaseNoiseActor.TRANSFORM_NAME.ToUpper());
            var rbnName = rbn.Path.ToString();

            // Create the RemoveSpikeNoise actor pool
            Props rspProps = Props.Create(() => new RemoveSpikeActor()).WithRouter(new RoundRobinPool(5));
            IActorRef rsp = vmstatsActorSystem.ActorOf(rspProps, "Transforms-" + RemoveSpikeActor.TRANSFORM_NAME.ToUpper());

            // Create the PercentizeActor actor pool
            Props pctProps = Props.Create(() => new PercentizeActor()).WithRouter(new RoundRobinPool(5));
            IActorRef pct = vmstatsActorSystem.ActorOf(pctProps, "Transforms-" + PercentizeActor.TRANSFORM_NAME.ToUpper());

            // Create the CombineTransformActor actor pool
            Props comProps = Props.Create(() => new CombineTransformActor()).WithRouter(new ConsistentHashingPool(5));
            IActorRef com = vmstatsActorSystem.ActorOf(comProps, "Transforms-" + CombineTransformActor.TRANSFORM_NAME.ToUpper());

            // Create the RemovePercentileActor actor pool
            Props rptProps = Props.Create(() => new RemovePercentileActor()).WithRouter(new RoundRobinPool(5));
            IActorRef rpt = vmstatsActorSystem.ActorOf(rptProps, "Transforms-" + RemovePercentileActor.TRANSFORM_NAME.ToUpper());

            // Create the RemovePercentileActor actor pool
            Props fldProps = Props.Create(() => new FlattenDeviationActor()).WithRouter(new RoundRobinPool(5));
            IActorRef fld = vmstatsActorSystem.ActorOf(fldProps, "Transforms-" + FlattenDeviationActor.TRANSFORM_NAME.ToUpper());

            // Create the RemoveLowValuesActor actor pool
            Props rlvProps = Props.Create(() => new RemoveLowValuesActor()).WithRouter(new RoundRobinPool(5));
            IActorRef rlv = vmstatsActorSystem.ActorOf(rlvProps, "Transforms-" + RemoveLowValuesActor.TRANSFORM_NAME.ToUpper());

            // Create the SaveMetricActor actor pool
            Props savProps = Props.Create(() => new SaveMetricActor()).WithRouter(new RoundRobinPool(5));
            IActorRef sav = vmstatsActorSystem.ActorOf(savProps, "Transforms-" + SaveMetricActor.TRANSFORM_NAME.ToUpper());

            // Create the ViewMetricActor actor pool
            Props viwProps = Props.Create(() => new ViewMetricActor()).WithRouter(new RoundRobinPool(5));
            IActorRef viw = vmstatsActorSystem.ActorOf(viwProps, "Transforms-" + ViewMetricActor.TRANSFORM_NAME.ToUpper());

            // Create the CompactTimeActor actor pool
            Props cmpProps = Props.Create(() => new CompactTimeActor()).WithRouter(new RoundRobinPool(5));
            IActorRef cmp = vmstatsActorSystem.ActorOf(cmpProps, "Transforms-" + CompactTimeActor.TRANSFORM_NAME.ToUpper());

            // Create the metric dispatcher
            Props metricAccumulatorDispatcherProps = Props.Create(() => new MetricAccumulatorDispatcherActor(snapshotPath, guiWebserUrl));
            IActorRef metricAccumulatorDispatcherActor = vmstatsActorSystem.ActorOf(metricAccumulatorDispatcherProps,
                MetricAccumulatorDispatcherActor.ACTOR_NAME);
            _log.Debug("Creating the metricAccumulatorDispatcherActor");

            // Create the actor that will watch the directory for new files being added
            Props directoryWatcherProps = Props.Create(() => new DirectoryWatcherActor(vmNamePattern, metricAccumulatorDispatcherActor));
            IActorRef directoryWatcherActor = vmstatsActorSystem.ActorOf(directoryWatcherProps,
                "directoryWatcherActor");
            _log.Debug("Creating the directoryWatcherActor");

            // Initialize the actor and then get it to check the directory for files
            directoryWatcherActor.Tell(new DirectoryWatcherActor.InitializeCommand(dirName, fileType));
            _log.Debug("Send DirectoryWatcherActor.InitializeCommand(dirName, fileType) message to directoryWatcherActor");

            // Schedule the DirectoryWatcher to check the directory
            vmstatsActorSystem.Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(30),
                          directoryWatcherActor, new DirectoryWatcherActor.CheckDirCommand(), ActorRefs.NoSender);
            _log.Debug("Scheduling the directoryWatcherActor with CheckDirCommand");

            // Create the MetricStoreManagerActor
            /*
            Props managerProps = Props.Create(() => new MetricStoreManagerActor(snapshotPath, guiWebserUrl));
            IActorRef managerActor = vmstatsActorSystem.ActorOf(managerProps,
                MetricStoreManagerActor.ACTOR_NAME);
            _log.Debug("Creating the MetricStoreManagerActor");
            */
            /*
            // Schedule the MetricStoreManager to check for new MetricStores
            vmstatsActorSystem.Scheduler
               .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                         TimeSpan.FromSeconds(30),
                          managerActor, new Messages.FindMetricStoreActorNames(), ActorRefs.NoSender);
            _log.Debug("Scheduling the MetricStoreManager with FindMetricStoreActorNames");
*/
        }
        private void GetEnvironmentVariables()
        {
            configFile = GetEnvironmentVariable(ENV_CONFIG_FILE);
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



        /*
                private static string GetConfiguration()
                {
                    string config = @"
                        akka {  
                            stdout-loglevel = INFO
                            loglevel = INFO
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
                */
    }
}
