using System;
using System.Linq;
using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Alexa.NET.SmartHome.IoC
{
    public static class Invoker
    {
        public static T InvokeAlexaMethod<T>(IConfiguration config, Header directiveHeader, string requestJson)
        {
            if(directiveHeader == null)
                throw new ArgumentException("The Amazon Alexa Directive must be specified.");

            //TODO: Force AbstractAlexaInterface
            var types = from t in ReflectionUtils.GetAllReferencedTypes()
                where t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.GetCustomAttributes(typeof(AlexaNamespaceAttribute), true).Length > 0)
                select t;

            foreach (var type in types)
            {
                var method = type.GetMethods().FirstOrDefault(m => m.Name == directiveHeader.Name);

                if (method != null)
                {
                    //TODO: Do this better
                    var paramType = method.GetParameters()[0].ParameterType;

                    var magicConstructor = type.GetConstructor(new[] {typeof(IConfiguration), typeof(string)});
                    var magicClassObject = magicConstructor.Invoke(new object[] { config, directiveHeader.Namespace });
                    return (T)method.Invoke(magicClassObject, new object[] { JsonConvert.DeserializeObject(requestJson, paramType) });
                }
            }

            throw new NotImplementedException($"This skill does not have a handler for the {directiveHeader.Namespace} {directiveHeader.Name} handler.");
        }
    }
}
