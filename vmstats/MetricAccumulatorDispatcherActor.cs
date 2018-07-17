using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;
using System.Collections;
using Akka.Event;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using vmstats_shared;

namespace vmstats
{
    class MetricName
    {
        public string name { get; set; }
    }


    internal class ActorID
    {
        public string VmName { get; private set; }
        public string Date { get; private set; }

        public ActorID(string vmName, string date)
        {
            VmName = vmName;
            Date = date;
        }
    }


    public class MetricAccumulatorDispatcherActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> routingTable = new Dictionary<string, IActorRef>();

        // The store for all the metrics found and their date ranges
        private Dictionary<string, SortedDictionary<long, IActorRef>> storeTable = new Dictionary<string, SortedDictionary<long, IActorRef>>();

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        
        // The regex pattern that defines the name of the snapshot files  
        private const string PATTERN = @"^snapshot-MetricStore%.*";
        private const string FILENAME_PATTERN = @"^snapshot-MetricStore%.*(?<vmname>V.*)%.*(?<date>\d{2}-\d{2}-\d{4})-(?<id>\d*)-\d*$";
        public const string ACTOR_NAME = "MetricAccumulatorDispatcher";

        // The place where the snapshots are stored
        private readonly string SnapshotPath;

        // THe URL of the server to send the results to
        private readonly string guiWebserverUrl;


        public MetricAccumulatorDispatcherActor(string snapshotPath, string guiWebserverUrl)
        {
            this.SnapshotPath = snapshotPath;
            this.guiWebserverUrl = guiWebserverUrl;

            Receive<Messages.MetricsToBeProcessed>(msg => Dispatch(msg));
            Receive<Messages.NoMoreMetrics>(msg => NoMoreMetrics());
            Receive<Messages.Stopping>(msg => Stopping());
            Receive<Messages.StartProcessingTransformPipeline>(msg => Process(msg));
            Receive<Messages.TransformSeries>(msg => ReturnResult(msg));
        }

        protected override void PreStart()
        {
            // Find out how many MetircStores have been created
            FindAvailableMetrics();

            // Log list of found stores
            var stores = JsonConvert.SerializeObject(storeTable);
            _log.Info($"Metric stores found: {stores}");
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


        private IActorRef CreateMetricStoreActor(string vmName, string date)
        {
            _log.Debug($"Create metric store actor for {vmName} with date {date}");

            // Create the actor
            var actor = Context.ActorOf(Props.Create(() =>
                new MetricStoreActor(vmName, date)), "MetricStore:" + vmName + ":" + date);

            // Add it to the list of metric actors being tracked
            AddMetricActor(vmName, date, actor);

            return actor;
        }


        private void Dispatch (Messages.MetricsToBeProcessed msg)
        {
            // Lookup the Actor to handle this request
            string key = "MetricAccumulator:" + msg.vmName + ":" + msg.date;
            IActorRef actor;
            if (!routingTable.TryGetValue(key, out actor))
            {
                // Create the actor to store the metrics in once they have been accumulated
                actor = CreateMetricStoreActor(msg.vmName, msg.date);

                // Create the actor for the accumulator and store its reference
                actor = Context.ActorOf(Props.Create(() =>
                    new MetricAccumulatorActor(msg.vmName, msg.date, actor)), key);
                routingTable.Add(key, actor);

                // Dispatch the message
                actor.Tell(msg);
            }
            else
            {
                actor.Tell(msg);
            }
        }


        private void AddMetricActor(string vmName, string vmDate, IActorRef iRef = null)
        {
            _log.Debug($"Add metric actor to store to keep track of the actors. Name {vmName} with date {vmDate} and ref {iRef}");
            
            // Save the information for use later
            SortedDictionary<long, IActorRef> dates;
            storeTable.TryGetValue(vmName, out dates);

            if (dates == null)
            {
                _log.Info($"Adding new metric actor for {vmName} with date {vmDate} and ref {iRef}");

                // New Actor!
                var sd = new SortedDictionary<long, IActorRef>();

                sd.Add(Convert.ToDateTime(vmDate).Ticks, iRef);
                storeTable.Add(vmName, sd);

            }
            else
            {
                // Check to see if the date is new
                if (dates.ContainsKey(Convert.ToDateTime(vmDate).Ticks))
                {
                    _log.Info($"Updating existing metric actor for {vmName} with date {vmDate} and ref {iRef}");
                    dates[Convert.ToDateTime(vmDate).Ticks] = iRef;
                }
                else
                {
                    _log.Info($"Adding date to existing metric actor for {vmName} with date {vmDate} and ref {iRef}");
                    dates.Add(Convert.ToDateTime(vmDate).Ticks, iRef);
                }
            }
        }


        private void Stopping()
        {
            string name = Context.Sender.Path.Name;
            routingTable.Remove(name);
        }


        private void NoMoreMetrics()
        {
            // Signal to all the accumulators that there are no more metrics
            foreach (var kvp in routingTable)
            {
                IActorRef actor = kvp.Value;
                actor.Tell(new Messages.NoMoreMetrics());
            }
        }


        private bool AddFileToAvailableMetrics(string filename)
        {
            // Extract the name of the metric store actor which includes the vmname and the date
            var match = Regex.Match(filename, FILENAME_PATTERN);

            if (match.Success)
            {
                // Extract the elements from the name of the file
                var vmName = match.Groups["vmname"].Value;
                var snapshotDate = match.Groups["date"].Value;

                // Add the actor to the set of known actors but set to a state reflecting that it once existed but is currently dormant
                AddMetricActor(vmName, snapshotDate);

                _log.Info($"Found file named: {filename} and added it to the known metric actors.");
                return true;
            }

            // If we get this far there was no match in the name of the file so return false
            _log.Info($"Found file named: {filename}, trying to extract vmname, date and id with pattern {FILENAME_PATTERN} but no match found.");
            return false;
        }


        private void Process(Messages.StartProcessingTransformPipeline msg)
        {
            _log.Info($"Processing pipeline.");

            Regex reg = new Regex(msg.cmd.VmPattern);
            long from = msg.cmd.FromDate.Ticks;
            long to = msg.cmd.ToDate.Ticks;

            // Find all of the MetricStoreActors that match the vmname and are in the date range
            var found = storeTable.ToList().Where
            (
                kvp => reg.IsMatch(kvp.Key)
            ).ToList();

            _log.Info($"Found {found.Count} actors that match the pattern of {msg.cmd.VmPattern}");

            // Find all those MetricStoreActors that match the dates
            var actorIDs = new List<IActorRef>();
            foreach (var vmname in found)
            {
                var dateKeys = vmname.Value.ToList();
                foreach (var date in dateKeys)
                {
                    if ((date.Key >= from) && (date.Key <= to))
                    {
                        var id = date.Value;
                        if (id == null)
                        {
                            var stringDate = (new DateTime(date.Key)).ToString("MM-dd-yyyy");

                            // Regenerate the actor so that it can process messages
                            _log.Info($"Regenerating MetricStoreActor named {vmname.Key} and date {stringDate}");
                            id = CreateMetricStoreActor(vmname.Key, stringDate);
                        }
                        actorIDs.Add(id);
                    }
                }
            }

            _log.Info($"Found {actorIDs.Count} metric stores with the correct name and match the dates from: {from} to: {to}.");

            // Now Tell each of the actors to process the request
            foreach (var id in actorIDs)
            {
                // Send the actor a msg for each of the transforms in the pipeline
                foreach (var series in msg.queue)
                {
                    // Pass the clients connection Id to the series so that it knows which client to send the results to
                    series.ConnectionId = msg.cmd.ConnectionId;

                    // Clone the series so they are distinct from one another
                    var newseries = series.Clone();

                    id.Tell(newseries);
                    _log.Debug($"Telling existing metric stored named: {id.Path} to start processing a transform pipeline: {JsonConvert.SerializeObject(series)}");
                }
            }
        }


        private async void ReturnResult(Messages.TransformSeries msg)
        {
            HttpClient client = null;

            var json = JsonConvert.SerializeObject(msg);
            _log.Debug($"Returning result to vmstatsGUI. Message received is: {json}");

            try
            {
                // Contact the vmstatsgui webserver and send it the details or the completed transform pipeline
                client = new HttpClient();
                client.BaseAddress = new Uri(guiWebserverUrl);

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

                // Create a new ProcessCommand with the supplied data
                // TODO plumb the vmname and date throughout the chain so it gets back to the client
                bool isRaw = (msg.GroupID == Guid.Empty) ? true : false;
                var result = new Messages.Result(msg.ConnectionId, msg.Measurements.Values.Keys.ToArray(), msg.Measurements.Values.Values.ToArray(), 
                    isRaw, msg.VmName, msg.VmDate, msg.Measurements.Name);
                string postBody = JsonConvert.SerializeObject(result);
                _log.Debug($"Returning result to vmstatsGUI. Result is: {postBody}");

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
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {hre.Message}. The URI used is {guiWebserverUrl}");
            }
            catch (TaskCanceledException tce)
            {
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {tce.Message}. The URI used is {guiWebserverUrl}");
            }
            catch (Exception ex)
            {
                _log.Error($"ERROR Calling vmstatsgui webserver. Error is: {ex.Message}. The URI used is {guiWebserverUrl}");
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


    }
}
