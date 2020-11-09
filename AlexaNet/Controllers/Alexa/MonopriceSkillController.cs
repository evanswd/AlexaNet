using Alexa.NET.SmartHome.Domain.Request;
using Microsoft.AspNetCore.Mvc;
using AlexaNet.Infrastructure.Amazon.Alexa;
using Newtonsoft.Json;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class MonopriceSkillController : ControllerBase
    {
        [HttpPost]
        public string Post(DirectiveRequest request)
        {
            //Gotta decide what we are doing here...
            if(request?.Directive?.Header?.Namespace =="Alexa.Discovery" && request.Directive?.Header?.Name == "Discover")
                return JsonConvert.SerializeObject(DiscoveryUtils.PerformDiscovery(request.Directive));

            return string.Empty;


            /*using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                System.IO.File.WriteAllText(DateTime.Now.Ticks + "_skill.log", x);
            }*/

            /*if (input?.Directive?.Header?.Namespace == "Alexa.Discovery")
                return ResponseBuilder.Tell($"It's working baby! {input.Directive.Header.Name}");
            else return ResponseBuilder.Tell("It still works!");*/

            /*System.IO.File.WriteAllText(DateTime.Now.Ticks + "_skill.log", JsonConvert.SerializeObject(input.Directive));
            return "";*/
        }
    }
}
