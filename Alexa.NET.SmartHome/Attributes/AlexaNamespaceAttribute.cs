using System;

namespace Alexa.NET.SmartHome.Attributes;

public class AlexaNamespaceAttribute : Attribute
{
    public readonly string Namespace;

    public AlexaNamespaceAttribute(string nameSpace)
    {
            Namespace = nameSpace;
        }
}