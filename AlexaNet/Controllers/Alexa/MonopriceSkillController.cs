using System.IO;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Alexa.NET.SmartHome.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class MonopriceSkillController : ControllerBase
    {
        private IConfiguration _config;

        public MonopriceSkillController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public async Task<EventResponse> Post()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var requestJson = await reader.ReadToEndAsync();

            var request = JsonConvert.DeserializeObject<DirectiveRequest>(requestJson);

            //Gotta decide what we are doing here...
            return Invoker.InvokeAlexaMethod<EventResponse>(_config, request?.Directive?.Header, requestJson);

            /*//If we get here, dump the input...
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var x = await reader.ReadToEndAsync();
            await System.IO.File.WriteAllTextAsync(DateTime.Now.Ticks + "_skill.log", x);
            
            return new EventResponse();*/


            /*if (input?.Directive?.Header?.Namespace == "Alexa.Discovery")
                return ResponseBuilder.Tell($"It's working baby! {input.Directive.Header.Name}");
            else return ResponseBuilder.Tell("It still works!");*/

            /*System.IO.File.WriteAllText(DateTime.Now.Ticks + "_skill.log", JsonConvert.SerializeObject(input.Directive));
            return "";*/
        }
    }
}
