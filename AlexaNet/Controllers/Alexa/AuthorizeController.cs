using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpPost]
        public async void Post()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                System.IO.File.WriteAllText(DateTime.Now.Ticks + "_auth.log", x);
            }
        }

        [HttpPost]
        [Route("Token")]
        public async void Token()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                System.IO.File.WriteAllText(DateTime.Now.Ticks + "_auth.log", x);
            }
        }
    }
}
