using Microsoft.AspNetCore.Mvc;
using System;
using System.Web;
using Newtonsoft.Json;

namespace AlexaNet.Controllers.Alexa
{
    [Route("Alexa/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpGet]
        public RedirectResult Get(string client_id, string response_type, string state, string scope, string redirect_uri)
        {
            //Yay, good for you... I trust you and you logged in!
            //TODO: Actually give a shit about logging in...
            var code = GetToken("code_");
            return Redirect(HttpUtility.UrlDecode(redirect_uri) + $"?state={state}&code={code}");
        }

        [HttpPost]
        [Route("Token")]
        public string Token(string grant_type, string code, string client_id, string redirect_uri)
        {
            //TODO: Actually care about providing a real token based on their code...
            return JsonConvert.SerializeObject(new
            {
                access_token = GetToken("access_"),
                token_type = "bearer",
                expires_in = 3600,
                refresh_token = GetToken("refresh_")
            });

            //NOTE: This is helpful debugging code!
            /*using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var x = await reader.ReadToEndAsync();
                System.IO.File.WriteAllText(DateTime.Now.Ticks + "_token.log", x);
            }*/
        }


        private string GetToken(string prefix)
        {
            return prefix + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        }
    }
}
