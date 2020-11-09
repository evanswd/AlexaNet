using System;
using System.Linq;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.IoC
{
    public static class Invoker
    {
        public static T InvokeAlexaMethod<T>(IConfiguration config, Header alexaHeader, Directive directive)
        {
            //TODO: Force AbstractAlexaInterface
            var types = from t in ReflectionUtils.GetAllReferencedTypes()
                where t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.GetCustomAttributes(typeof(AlexaNamespaceAttribute), true).Length > 0)
                select t;

            foreach (var type in types)
            {
                var method = type.GetMethods().FirstOrDefault(m => m.Name == alexaHeader.Name);
                if (method != null)
                {
                    var magicConstructor = type.GetConstructor(new[] {typeof(IConfiguration)});
                    var magicClassObject = magicConstructor.Invoke(new object[] { config });
                    return (T)method.Invoke(magicClassObject, new object[] { directive });
                }
            }

            throw new NotImplementedException($"This skill does not have a handler for the {alexaHeader.Namespace} {alexaHeader.Name} handler.");
        }
    }
}
