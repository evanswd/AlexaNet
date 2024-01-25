using Alexa.NET.SmartHome.Attributes;
using Alexa.NET.SmartHome.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Alexa.NET.SmartHome.IoC;

public static class Invoker
{
    //Used if the 
    private static readonly object LockObject = new();

    public static T InvokeAlexaMethod<T>(IConfiguration config, Header directiveHeader, string requestJson, ILogger logger)
    {
            logger.LogWarning($"Got a request for {directiveHeader.Namespace} to complete {directiveHeader.Name} action.");
            logger.LogWarning(requestJson);

            if (directiveHeader == null)
                throw new ArgumentException("The Amazon Alexa Directive must be specified.");

            //Find all classes that are decorated with [AlexaNamespace] and match the request's Directive Header
            var types = from t in ReflectionUtils.GetAllReferencedTypes()
                        where t.IsClass && !t.IsAbstract && t.GetInterfaces()
                            .Any(i => i.GetCustomAttributes(typeof(AlexaNamespaceAttribute), true)
                                .Any(ca => ((AlexaNamespaceAttribute)ca).Namespace == directiveHeader.Namespace))
                        select t;

            //TODO: Do this better... how to handle multiple classes? This only handles the first one that comes back...
            //For example, if both a SpeakerZone and a LightingZone implement 'Alexa.PowerController' we will have an issue...
            foreach (var type in types)
            {
                var method = type.GetMethods().FirstOrDefault(m => m.Name == directiveHeader.Name);
                //If this type doesn't have the method name we need, move on...
                if (method == null) continue;

                var paramType = method.GetParameters()[0].ParameterType;
                var magicConstructor = type.GetConstructor(new[] { typeof(IConfiguration), typeof(string) });

                //If the class requires a lock, handle accordingly...
                //This is particularly because of things like serial connections that can't run async
                if (type.GetCustomAttributes(typeof(RequiresLockAttribute), true).Length > 0)
                    lock (LockObject)
                    {
                        logger.LogWarning($"{type.FullName}.{method.Name} is requiring a lock.");
                        var magicClassObject = magicConstructor.Invoke(new object[] { config, directiveHeader.Namespace });
                        var result = (T)method.Invoke(magicClassObject, new[] { JsonConvert.DeserializeObject(requestJson, paramType) });
                        logger.LogWarning($"Finished Execution for {directiveHeader.Namespace} to complete {directiveHeader.Name} action.");
                        logger.LogWarning(JsonConvert.SerializeObject(result));
                        return result;
                    }
                else
                {
                    logger.LogWarning($"{type.FullName}.{method.Name} is NOT requiring a lock.");
                    var magicClassObject = magicConstructor.Invoke(new object[] { config, directiveHeader.Namespace });
                    var result = (T)method.Invoke(magicClassObject, new[] { JsonConvert.DeserializeObject(requestJson, paramType) });
                    logger.LogWarning($"Finished Execution for {directiveHeader.Namespace} to complete {directiveHeader.Name} action.");
                    logger.LogWarning(JsonConvert.SerializeObject(result));
                    return result;
                }
            }

            throw new NotImplementedException($"This skill does not have a handler for the {directiveHeader.Namespace} {directiveHeader.Name} handler.");
        }
}