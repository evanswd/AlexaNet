using Microsoft.AspNetCore.Mvc;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Authorization;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class MonopriceSkillController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public SkillResponse Get(SkillRequest input)
        {
            if (input.Request is IntentRequest)
                return ResponseBuilder.Tell($"It's working baby! {((IntentRequest) input.Request).Intent.Name}");
            else return ResponseBuilder.Tell("It still works!");
        }
    }
}
