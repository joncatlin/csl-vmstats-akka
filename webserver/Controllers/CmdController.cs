using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using vmstats.lang;
using vmstats;
using vmstats_shared;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace webserver.Controllers
{
    [Route("api/cmd")]
    [ApiController]
    public class CmdController : ControllerBase
    {
        private readonly ILogger<CmdController> _log;

        // Get a reference to the Startup class which holds the actor system initialization
        static StartActors startup = StartActors.Instance;

        public CmdController(ILogger<CmdController> log)
        {
            _log = log;
        }

        // GET api/cmd
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            _log.LogDebug("Received request Get");
            return new string[] { "value1", "value2" };
        }


        // GET api/cmd/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

//        [HttpPost(Name = nameof(ValidateRequest))]
        [HttpPost]
        public ActionResult<CreatedAtRouteResult> Post([FromBody] Messages.ProcessCommand request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            _log.LogDebug($"Received request Post. Request is: {jsonRequest}");
            try
            {
                // Validate the dsl
                Queue<Messages.BuildTransformSeries> q = new Queue<Messages.BuildTransformSeries>();
                TransformationLanguage tl = new TransformationLanguage(startup._log);
                tl.DecodeAndExecute(request.Dsl, q);

                // Start the processing of the pipeline by telling the MetricAccumulatorDispatcherActor to route the request
                var actor = startup.vmstatsActorSystem.ActorSelection("/user/" + MetricAccumulatorDispatcherActor.ACTOR_NAME);
                actor.Tell(new Messages.StartProcessingTransformPipeline(request, q));
                startup.vmstatsActorSystem.Log.Debug("Starting the processing of the transforms specified in the dsl");
                Console.WriteLine("Starting the processing of the transforms specified in the dsl");
            }
            catch (VmstatsLangException e)
            {
                startup.vmstatsActorSystem.Log.Error($"Error processing DSL in request. Message is: {e.Message}");
                var errorMsg = $"Error processing DSL in request. Message: {e.Message}";
                _log.LogError(errorMsg);
                Console.WriteLine(errorMsg);
                return BadRequest(errorMsg);
            } catch (Exception e)
            {
                startup.vmstatsActorSystem.Log.Error($"Error processing DSL in request. Message is: {e.Message}");
                var errorMsg = $"Error processing DSL in request. Message: {e.Message}";
                _log.LogError(errorMsg);
                Console.WriteLine(errorMsg);
                return BadRequest(errorMsg);
            }

            return Ok();
        }


















        // PUT api/cmd/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/cmd/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
