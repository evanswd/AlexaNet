using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Domain.Response;
using Microsoft.AspNetCore.Mvc;
using AlexaNet.Infrastructure.Amazon.Alexa;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class MonopriceSkillController : ControllerBase
    {
        [HttpPost]
        public async Task<EventResponse> Post(DirectiveRequest request)
        {
            //Gotta decide what we are doing here...
            if(request?.Directive?.Header?.Namespace =="Alexa.Discovery" && request.Directive?.Header?.Name == "Discover")
                return DiscoveryUtils.PerformDiscovery(request.Directive);

            //If we get here, dump the input...
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var x = await reader.ReadToEndAsync();
            await System.IO.File.WriteAllTextAsync(DateTime.Now.Ticks + "_token.log", x);
            return null;


            /*if (input?.Directive?.Header?.Namespace == "Alexa.Discovery")
                return ResponseBuilder.Tell($"It's working baby! {input.Directive.Header.Name}");
            else return ResponseBuilder.Tell("It still works!");*/

            /*System.IO.File.WriteAllText(DateTime.Now.Ticks + "_skill.log", JsonConvert.SerializeObject(input.Directive));
            return "";*/
        }
    }
}
