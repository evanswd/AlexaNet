using Microsoft.AspNetCore.Mvc;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Authorization;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class SpeakerSystemController : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public SkillResponse Get(SkillRequest input)
        {
            return ResponseBuilder.Tell($"It's working baby!");
        }
    }
}
