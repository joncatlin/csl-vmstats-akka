using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Newtonsoft.Json;

namespace vmstats
{
    class MetricStoreManagerActor : ReceiveActor
    {
        // The regex pattern that defines the name of the snapshot files  
        private const string PATTERN = @"^snapshot-MetricStore%.*";
        private const string FILENAME_PATTERN = @"^snapshot-MetricStore%(.*)%.*(?<vmname>V.*)%.*(?<date>\d{2}-\d{2}-\d{4})-(?<id>\d*)-\d*$";

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        // The store for all the metrics found and their date ranges
        private Dictionary<string, SortedDictionary<long, string>> AvailableMetrics = new Dictionary<string, SortedDictionary<long, string>>();

        // The place where the snapshots are stored
        private readonly string SnapshotPath;

        protected override void PreStart()
        {
            // Find out how many MetircStores have been created
            FindAvailableMetrics();

            // Start a filewatcher on the snapshot directory to be notified whenever a new file is created
            StartFileWatcher();

            // Log list of found stores
            var stores = JsonConvert.SerializeObject(AvailableMetrics);
            _log.Info($"Metric stores found: {stores}");
        }


        public MetricStoreManagerActor(string snapshotPath)
        {
            this.SnapshotPath = snapshotPath;

            Receive<Messages.ProcessCommand>(msg => Process(msg));
        }


        private void Process(Messages.ProcessCommand msg)
        {
            Regex reg = new Regex(msg.VmPattern);
            long from = msg.FromDate.Ticks;
            long to = msg.ToDate.Ticks;

            // Find all of the MetricStoreActors that match the vmname and are in the date range
            var found = AvailableMetrics.ToList().Where
            (
                kvp => reg.IsMatch(kvp.Key)
            ).ToList();

            // Find all those MetricStoreActors that match the dates
            List<string> actorNames = new List<string>();
            foreach (var vmname in found)
            {
                foreach (var date in vmname.Value)
                {
                    if ((date.Key >= from) && (date.Key <= to))
                    {
                        long ticks = date.Key;
                        DateTime dt = new DateTime(ticks);
                        string dateString = dt.ToString("MM-dd-yyyy");
                        var name = vmname.Key + "-" + dateString;
                        actorNames.Add(name);
                    }
                }
            }
            int i = 0;
        }


        private void FindAvailableMetrics()
        {
            Regex reg = new Regex(PATTERN);

            // Find all the files in the directory that match the regular expression
            var files = Directory.GetFiles(SnapshotPath, "*").ToList()
                                 .Where(path => reg.IsMatch(Path.GetFileName(path)))
                                 .ToList();

            // Add each file found to the list of available metrics
            foreach (string file in files)
            {
                AddFileToAvailableMetrics(Path.GetFileName(file));
            }
        }


        private bool AddFileToAvailableMetrics (string filename)
        {
            // Extract the name of the metric store actor which includes the vmname and the date
            var match = Regex.Match(filename, FILENAME_PATTERN);

            if (match.Success)
            {
                // Extract the elements from the name of the file
                var vmName = match.Groups["vmname"].Value;
                var snapshotDate = match.Groups["date"].Value;
                var snapshotId = match.Groups["id"].Value;

                // Save the information for use later
                SortedDictionary<long, string> dates;
                AvailableMetrics.TryGetValue(vmName, out dates);

                if (dates == null)
                {
                    // Create an entry for the vm and date
                    long dateLong = Convert.ToDateTime(snapshotDate).Ticks;
                    var sd = new SortedDictionary<long, string>();
                    sd.Add(Convert.ToDateTime(snapshotDate).Ticks, snapshotId);
                    AvailableMetrics.Add(vmName, sd);
                }
                else
                {
                    // Add the date the tthe existing vmname set of them
                    dates.TryAdd(Convert.ToDateTime(snapshotDate).Ticks, snapshotId);
                }

                _log.Info($"Found file named: {filename} and added it to the AvailableMetrics.");
                return true;
            }

            // If we get this far there was no match in the name of the file so return false
            _log.Info($"Found file named: {filename}, trying to extract vmname, date and id with pattern {FILENAME_PATTERN} but no match found.");
            return false;
        }


        private void StartFileWatcher() { 
            var fileSystemWatcher = new FileSystemWatcher();

            // Associate event handlers with the events
            fileSystemWatcher.Created += FileSystemWatcher_Created;
//            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
//            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
//            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            // Set the path of the directory to watch
            fileSystemWatcher.Path = SnapshotPath;
 
            // Start event notification
            fileSystemWatcher.EnableRaisingEvents = true;
        }
/*
        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine("Hello");
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Hello");
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Hello");
        }
*/
        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            _log.Info($"New file detected in snapshot directory. File name is: {e.Name}.");
            AddFileToAvailableMetrics(e.Name);
        }

        static void Main(string[] args)
        {
            // Create the container for all the actors
            var vmstatsActorSystem = ActorSystem.Create("vmstats");

            // Create the metricstoremanager
            Props managerProps = Props.Create(() => new MetricStoreManagerActor(@"C:\temp\snapshots"));
            IActorRef managerActor = vmstatsActorSystem.ActorOf(managerProps,
                "MetricStoreManagerActor");

            // Initialize the actor and then get it to check the directory for files
            //            managerActor.Tell(new DirectoryWatcherActor.InitializeCommand(dirName, fileType));

            // Schedule the DirectoryWatcher to check the directory
                        vmstatsActorSystem.Scheduler
                            .ScheduleTellRepeatedly(TimeSpan.FromSeconds(0),
                                        TimeSpan.FromSeconds(30),
                                        managerActor, DirectoryWatcherActor.CheckDirCommand, ActorRefs.NoSender);

            // Send a ProcessCommand for a particular vmpattern and date
            var msg = new Messages.ProcessCommand();
            msg.FromDate = Convert.ToDateTime("01-16-2018");
            msg.ToDate = Convert.ToDateTime("01-18-2018");
            msg.Dsl = "MAXCPU->RBN:RNP";
            msg.VmPattern = "V-JCatlin|V-BWill";

            managerActor.Tell(msg);

            // Wait until actor system terminated
            vmstatsActorSystem.WhenTerminated.Wait();
        }
    }
}
