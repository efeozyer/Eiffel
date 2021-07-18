using Autofac;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Eiffel.Web.AspNetCore
{
    public static class AutofacModuleExtensions
    {
        public static void AddModules(this ContainerBuilder builder, string searchPattern, Assembly[] assemblies = null)
        {
            if (assemblies == null)
            {
                assemblies = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, searchPattern, SearchOption.TopDirectoryOnly)
                    .Select(Assembly.LoadFrom)
                    .ToArray();

                builder.RegisterAssemblyModules(assemblies);
            }
        }
    }
}
