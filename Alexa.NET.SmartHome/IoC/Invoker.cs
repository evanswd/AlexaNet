using System;
using System.Linq;
using Alexa.NET.SmartHome.Domain.Request;
using Alexa.NET.SmartHome.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Alexa.NET.SmartHome.IoC
{
    public static class Invoker
    {
        public static T InvokeAlexaMethod<T>(IConfiguration config, DirectiveRequest directiveRequest)
        {
            if(directiveRequest?.Directive?.Header == null)
                throw new ArgumentException("The Amazon Alexa Directive must be specified.");

            //TODO: Force AbstractAlexaInterface
            var types = from t in ReflectionUtils.GetAllReferencedTypes()
                where t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.GetCustomAttributes(typeof(AlexaNamespaceAttribute), true).Length > 0)
                select t;

            foreach (var type in types)
            {
                var method = type.GetMethods().FirstOrDefault(m => m.Name == directiveRequest?.Directive?.Header.Name);
                if (method != null)
                {
                    var magicConstructor = type.GetConstructor(new[] {typeof(IConfiguration), typeof(string)});
                    var magicClassObject = magicConstructor.Invoke(new object[] { config, directiveRequest?.Directive?.Header.Namespace });
                    return (T)method.Invoke(magicClassObject, new object[] { directiveRequest?.Directive });
                }
            }

            throw new NotImplementedException($"This skill does not have a handler for the {directiveRequest?.Directive?.Header.Namespace} {directiveRequest?.Directive?.Header.Name} handler.");
        }
    }
}
