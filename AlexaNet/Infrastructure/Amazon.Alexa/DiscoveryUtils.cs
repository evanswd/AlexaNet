using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Response;

namespace AlexaNet.Infrastructure.Amazon.Alexa
{
    public static class DiscoveryUtils
    {
        //To Be Determined How this works... but it should find all my Alexa "stuff"
        public static EventResponse PerformDiscovery(Directive directive)
        {
            var response = new EventResponse
            {
                Event = new Directive
                {
                    Header = directive.Header,
                    Payload = new Payload
                    {
                        Endpoints = new[]
                        {
                            new Endpoint
                            {
                                EndpointID = "Zone03", //Careful... alphanumeric only... BAD documentation...
                                ManufacturerName = "Monoprice",
                                Description = "Not-so-smart Speaker by Bill Evans",
                                FriendlyName = "Office Speakers",
                                DisplayCategories = new [] {DisplayCategories.SPEAKER},
                                Capabilities = new []
                                {
                                    //new Capability("Alexa"), 
                                    new Capability("Alexa.Speaker", "volume", "muted"),
                                    new Capability("Alexa.PowerController", "powerState")
                                }
                            }
                        }
                    }
                }
            };

            //Fix the header...
            response.Event.Header.Name = "Discover.Response";
            //Kick it back
            return response;
        }
    }
}
