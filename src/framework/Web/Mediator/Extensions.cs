using FSH.Framework.Web.Mediator.Behaviors;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FSH.Framework.Web.Mediator;
public static class Extensions
{
    public static IServiceCollection
        EnableMediator(this IServiceCollection services, params Assembly[] featureAssemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (featureAssemblies is null || featureAssemblies.Length == 0)
            featureAssemblies = [Assembly.GetExecutingAssembly()];

        var assemblyReferences = new List<AssemblyReference>();

        foreach (var assembly in featureAssemblies)
        {
            assemblyReferences.Add(assembly);
        }

        services.AddMediator(o =>
        {
            o.ServiceLifetime = ServiceLifetime.Transient;
            o.Assemblies = assemblyReferences;
        });

        // Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

}
