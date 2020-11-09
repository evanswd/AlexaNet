using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.Interfaces
{
    public abstract class AbstractSmartHomeInterface
    {
        protected IConfiguration Config;
        protected readonly string AlexaNamespace;

        public AbstractSmartHomeInterface(IConfiguration config, string alexaNamespace)
        {
            Config = config;
            AlexaNamespace = alexaNamespace;
        }
    }
}
