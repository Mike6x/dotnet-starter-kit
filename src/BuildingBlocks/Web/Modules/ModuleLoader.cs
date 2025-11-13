// FSH.Framework.Web/Modules/ModuleLoader.cs
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FSH.Framework.Web.Modules;

public static class ModuleLoader
{
    private static readonly List<IModule> _modules = new();
    private static readonly object _lock = new();
    private static bool _modulesLoaded;

    public static IHostApplicationBuilder AddModules(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        lock (_lock)
        {
            if (_modulesLoaded)
            {
                return builder;
            }

            var source = assemblies is { Length: > 0 }
                ? assemblies
                : AppDomain.CurrentDomain.GetAssemblies();

            var moduleRegistrations = source
                .SelectMany(a => a.GetCustomAttributes<FshModuleAttribute>());

            foreach (var registration in moduleRegistrations
                .Where(r => typeof(IModule).IsAssignableFrom(r.ModuleType))
                .DistinctBy(r => r.ModuleType)
                .OrderBy(r => r.Order)
                .ThenBy(r => r.ModuleType.Name))
            {
                if (Activator.CreateInstance(registration.ModuleType) is not IModule module)
                {
                    throw new InvalidOperationException($"Unable to create module {registration.ModuleType.Name}.");
                }

                module.ConfigureServices(builder);
                _modules.Add(module);
            }

            _modulesLoaded = true;
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapModules(this IEndpointRouteBuilder endpoints)
    {
        foreach (var m in _modules)
            m.MapEndpoints(endpoints);

        return endpoints;
    }
}
