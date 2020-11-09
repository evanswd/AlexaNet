using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Alexa.NET.SmartHome.IoC
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Assembly> GetAllReferencedAssemblies()
        {
            //TODO: This still may be an issue if dlls are referenced from the GAC (or anywhere outside the bin)
            //First, force all assemblies in the bin to load...
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Where(asm => !asm.IsDynamic).Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Type> GetAllReferencedTypes()
        {

            return GetAllReferencedAssemblies().SelectMany(asm => asm.GetTypes());
        }
    }
}
