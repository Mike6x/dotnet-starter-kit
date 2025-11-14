using FSH.Modules.Auditing;
using FSH.Modules.Identity;
using FSH.Modules.Multitenancy;
using NetArchTest.Rules;
using Shouldly;
using Xunit;

namespace Architecture.Tests;

public class PlaygroundArchitectureTests
{
    [Fact]
    public void Modules_Should_Not_Depend_On_Playground_Hosts()
    {
        var moduleAssemblies = new[]
        {
            typeof(AuditingModule).Assembly,
            typeof(IdentityModule).Assembly,
            typeof(MultitenancyModule).Assembly
        };

        // Assemblies / namespaces that represent Playground hosts.
        string[] playgroundNamespaces =
        {
            "FSH.Playground.Api",
            "Playground.Blazor"
        };

        foreach (var moduleAssembly in moduleAssemblies)
        {
            var result = Types
                .InAssembly(moduleAssembly)
                .Should()
                .NotHaveDependencyOnAny(playgroundNamespaces)
                .GetResult();

            result.IsSuccessful.ShouldBeTrue(
                $"Module '{moduleAssembly.FullName}' should not depend on Playground host assemblies.");
        }
    }
}

internal static class ModuleArchitectureTestsFixture
{
    public static readonly string SolutionRoot = GetSolutionRoot();

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "src")))
        {
            directory = directory.Parent;
        }

        if (directory is null)
        {
            throw new InvalidOperationException("Unable to locate solution root containing 'src' folder.");
        }

        return directory.FullName;
    }
}
