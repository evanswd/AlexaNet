using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.Interfaces
{
    public abstract class AbstractSmartHomeInterface
    {
        protected IConfiguration Config;

        public AbstractSmartHomeInterface(IConfiguration config)
        {
            Config = config;
        }
    }
}
