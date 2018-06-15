using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vmstats.lang;
using transforms;
using vmstats;

namespace webserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CmdController : ControllerBase
    {
        // Get a reference to the Startup class which holds the actor system initialization
        static StartActors startup = StartActors.Instance;

        // GET api/cmd
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
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
        public ActionResult<CreatedAtRouteResult> Post([FromBody] vmstats.Messages.ProcessCommand request)
        {
            try
            {
                // Validate the dsl
                Queue<BuildTransformSeries> q = new Queue<BuildTransformSeries>();
                TransformationLanguage tl = new TransformationLanguage(startup._log);
                tl.DecodeAndExecute(request.Dsl, q);

                // TODO get the valid result and kick off the processing
                Console.WriteLine("Starting the processing");

            } catch (VmstatsLangException e)
            {
                return BadRequest("Error processing DSL in request. Message: " + e.Message);
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
