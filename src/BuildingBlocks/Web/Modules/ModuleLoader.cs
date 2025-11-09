// FSH.Framework.Web/Modules/ModuleLoader.cs
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace FSH.Framework.Web.Modules;

public static class ModuleLoader
{
    private static readonly List<IModule> _modules = new();

    public static IHostApplicationBuilder AddModules(this IHostApplicationBuilder builder, params Assembly[] assemblies)
    {
        var source = assemblies is { Length: > 0 } ? assemblies : AppDomain.CurrentDomain.GetAssemblies();

        foreach (var type in source.SelectMany(a => a.DefinedTypes))
        {
            if (type.IsAbstract || !typeof(IModule).IsAssignableFrom(type)) continue;

            if (Activator.CreateInstance(type) is IModule module)
            {
                module.ConfigureServices(builder);
                _modules.Add(module);
            }
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
