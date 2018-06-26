using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Newtonsoft.Json;

namespace vmstats
{
    public class MetricStoreManagerActor : ReceiveActor
    {
        // The regex pattern that defines the name of the snapshot files  
        private const string PATTERN = @"^snapshot-MetricStore%.*";
        private const string FILENAME_PATTERN = @"^snapshot-MetricStore%.*(?<vmname>V.*)%.*(?<date>\d{2}-\d{2}-\d{4})-(?<id>\d*)-\d*$";
        public const string ACTOR_NAME = "MetricStoreManager";

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        // The store for all the metrics found and their date ranges
        private Dictionary<string, SortedDictionary<long, string>> AvailableMetrics = new Dictionary<string, SortedDictionary<long, string>>();

        // The place where the snapshots are stored
        private readonly string SnapshotPath;
        private readonly string guiWebserverUrl;

        protected override void PreStart()
        {
            // Find out how many MetircStores have been created
            FindAvailableMetrics();

            // Start a filewatcher on the snapshot directory to be notified whenever a new file is created
//            StartFileWatcher();

            // Log list of found stores
            var stores = JsonConvert.SerializeObject(AvailableMetrics);
            _log.Info($"Metric stores found: {stores}");
        }


        public MetricStoreManagerActor(string snapshotPath, string guiWebserverUrl)
        {
            this.SnapshotPath = snapshotPath;
            this.guiWebserverUrl = guiWebserverUrl;

            Receive<Messages.StartProcessingTransformPipeline>(msg => Process(msg));
            Receive<Messages.FindMetricStoreActorNames>(msg => Find(msg));
            Receive<Messages.TransformSeries>(msg => ReturnResult(msg));
        }

        private async void ReturnResult(Messages.TransformSeries msg)
        {
            HttpClient client = null;


            var json = JsonConvert.SerializeObject(msg);
            _log.Debug($"Returning result to vmstatsGUI. Result is: {json}");

            try
            {
                // Contact the vmstatsgui webserver and send it the details or the completed transform pipeline
                client = new HttpClient();
                client.BaseAddress = new Uri(guiWebserverUrl);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // Create a new ProcessCommand with the supplied data
                string postBody = JsonConvert.SerializeObject(msg);

                // Send the results to the vmstatsGUI
                HttpResponseMessage response = await client.PostAsync(guiWebserverUrl, new StringContent(postBody, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    _log.Info("Successfully returned the result of a transform seriues to the vmstatsGUI.");
                }
                else
                {
                    _log.Error($"Failed to ReturnResult to vmstatsGUI. Reason: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException hre)
            {
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {hre.Message}");
            }
            catch (TaskCanceledException tce)
            {
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {tce.Message}");
            }
            catch (Exception ex)
            {
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {ex.Message} ");
            }
            finally
            {
                if (client != null)
                {
                    client.Dispose();
                    client = null;
                }
            }
        }

        private void Find(Messages.FindMetricStoreActorNames msg)
        {
            _log.Info($"Processing FindMetricStoreActorNames");
            FindAvailableMetrics();
            var json = JsonConvert.SerializeObject(AvailableMetrics);
            _log.Debug($"Processed FindMetricStoreActorNames msg. AvailableMetrics found: {json}");
        }

        internal class ActorID{
            public string VmName { get; private set; }
            public string Date { get; private set; }

            public ActorID(string vmName, string date)
            {
                VmName = vmName;
                Date = date;
            }
        }

        private void Process(Messages.StartProcessingTransformPipeline msg)
        {
            Regex reg = new Regex(msg.cmd.VmPattern);
            long from = msg.cmd.FromDate.Ticks;
            long to = msg.cmd.ToDate.Ticks;

            // Find all of the MetricStoreActors that match the vmname and are in the date range
            var found = AvailableMetrics.ToList().Where
            (
                kvp => reg.IsMatch(kvp.Key)
            ).ToList();

            // Find all those MetricStoreActors that match the dates
            List<ActorID> actorNames = new List<ActorID>();
            foreach (var vmname in found)
            {
                foreach (var date in vmname.Value)
                {
                    if ((date.Key >= from) && (date.Key <= to))
                    {
                        long ticks = date.Key;
                        DateTime dt = new DateTime(ticks);
                        string dateString = dt.ToString("MM-dd-yyyy");
                        var id = new ActorID(vmname.Key, dateString);
                        actorNames.Add(id);
                    }
                }
            }

            // Now Tell each of the actors to process the request
            foreach (var id in actorNames)
            {
                var actorName = "MetricStore:" + id.VmName + ":" + id.Date;
                _log.Debug($"Begin sending msgs to actor named: {actorName}");

                // Send the actor a msg for each of the transforms in the pipeline
                foreach (var series in msg.queue)
                {
                    // Pass the clients connection Id to the series so that it know why client to send the results to
                    series.ConnectionId = msg.cmd.ConnectionId;

                    try
                    {
                        // The Metric Store may not be active so activeate it
                        var actor = Context.ActorOf(Props.Create(() =>
                            new MetricStoreActor(id.VmName, id.Date)), actorName);

                        _log.Debug($"Telling new metric stored named: {actorName} to start processing a transform pipeline: {JsonConvert.SerializeObject(series)}");
                        actor.Tell(series);
                    }
                    catch (InvalidActorNameException)
                    {
                        // IGNORE THIS. If the actor already exists then get its context by looking it up and sending it the messge.
                        var foundActor = Context.ActorSelection("**/" + actorName);
                        foundActor.Tell(series);
                        _log.Debug($"Telling existing metric stored named: {actorName} to start processing a transform pipeline: {JsonConvert.SerializeObject(series)}");
                    }
                    catch (Exception)
                    {
                        _log.Error($"Execption thrown while trying to communicate to actor: {actorName}. Pipeline: {JsonConvert.SerializeObject(series)}");
                    }
                }
            }
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

        /*
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

                private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
                {
                    _log.Info($"New file detected in snapshot directory. File name is: {e.Name}.");
                    AddFileToAvailableMetrics(e.Name);
                }
                */

    }
}
