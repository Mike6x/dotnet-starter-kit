using System.Diagnostics;
using FSH.CLI.Models;
using FSH.CLI.UI;
using Spectre.Console;

namespace FSH.CLI.Scaffolding;

internal static class SolutionGenerator
{
    public static async Task GenerateAsync(ProjectOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var projectPath = Path.Combine(options.OutputPath, options.Name);

        if (Directory.Exists(projectPath) &&
            Directory.EnumerateFileSystemEntries(projectPath).Any() &&
            !AnsiConsole.Confirm($"Directory [yellow]{projectPath}[/] is not empty. Continue anyway?", false))
        {
            AnsiConsole.MarkupLine("[yellow]Aborted.[/]");
            return;
        }

        AnsiConsole.WriteLine();

        await AnsiConsole.Progress()
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                var mainTask = ctx.AddTask("[green]Creating project...[/]");

                // Create directory structure
                var structureTask = ctx.AddTask("Creating directory structure");
                await CreateDirectoryStructureAsync(projectPath, options);
                structureTask.Increment(100);

                // Create solution file
                var solutionTask = ctx.AddTask("Creating solution file");
                await CreateSolutionFileAsync(projectPath, options);
                solutionTask.Increment(100);

                // Create API project
                var apiTask = ctx.AddTask("Creating API project");
                await CreateApiProjectAsync(projectPath, options);
                apiTask.Increment(100);

                // Create Blazor project if needed
                if (options.Type == ProjectType.ApiBlazor)
                {
                    var blazorTask = ctx.AddTask("Creating Blazor project");
                    await CreateBlazorProjectAsync(projectPath, options);
                    blazorTask.Increment(100);
                }

                // Create migrations project
                var migrationsTask = ctx.AddTask("Creating migrations project");
                await CreateMigrationsProjectAsync(projectPath, options);
                migrationsTask.Increment(100);

                // Create AppHost if Aspire enabled
                if (options.IncludeAspire)
                {
                    var aspireTask = ctx.AddTask("Creating Aspire AppHost");
                    await CreateAspireAppHostAsync(projectPath, options);
                    aspireTask.Increment(100);
                }

                // Create Docker Compose if enabled
                if (options.IncludeDocker)
                {
                    var dockerTask = ctx.AddTask("Creating Docker Compose");
                    await CreateDockerComposeAsync(projectPath, options);
                    dockerTask.Increment(100);
                }

                // Create sample module if enabled
                if (options.IncludeSampleModule)
                {
                    var sampleTask = ctx.AddTask("Creating sample module");
                    await CreateSampleModuleAsync(projectPath, options);
                    sampleTask.Increment(100);
                }

                // Create Terraform if enabled
                if (options.IncludeTerraform)
                {
                    var terraformTask = ctx.AddTask("Creating Terraform files");
                    await CreateTerraformAsync(projectPath, options);
                    terraformTask.Increment(100);
                }

                // Create GitHub Actions if enabled
                if (options.IncludeGitHubActions)
                {
                    var ciTask = ctx.AddTask("Creating GitHub Actions");
                    await CreateGitHubActionsAsync(projectPath, options);
                    ciTask.Increment(100);
                }

                // Create common files
                var commonTask = ctx.AddTask("Creating common files");
                await CreateCommonFilesAsync(projectPath, options);
                commonTask.Increment(100);

                mainTask.Increment(100);
            });

        AnsiConsole.WriteLine();

        // Run dotnet restore
        await RunDotnetRestoreAsync(projectPath, options);

        // Show next steps
        ShowNextSteps(options);
    }

    private static Task CreateDirectoryStructureAsync(string projectPath, ProjectOptions options)
    {
        var directories = new List<string>
        {
            "src",
            $"src/{options.Name}.Api",
            $"src/{options.Name}.Api/Properties",
            $"src/{options.Name}.Migrations"
        };

        if (options.Type == ProjectType.ApiBlazor)
        {
            directories.Add($"src/{options.Name}.Blazor");
            directories.Add($"src/{options.Name}.Blazor/Pages");
            directories.Add($"src/{options.Name}.Blazor/Shared");
            directories.Add($"src/{options.Name}.Blazor/wwwroot");
        }

        if (options.IncludeAspire)
        {
            directories.Add($"src/{options.Name}.AppHost");
            directories.Add($"src/{options.Name}.AppHost/Properties");
        }

        if (options.IncludeSampleModule)
        {
            directories.Add($"src/Modules/{options.Name}.Catalog");
            directories.Add($"src/Modules/{options.Name}.Catalog.Contracts");
        }

        if (options.IncludeTerraform)
        {
            directories.Add("terraform");
        }

        if (options.IncludeGitHubActions)
        {
            directories.Add(".github/workflows");
        }

        foreach (var dir in directories)
        {
            Directory.CreateDirectory(Path.Combine(projectPath, dir));
        }

        return Task.CompletedTask;
    }

    private static async Task CreateSolutionFileAsync(string projectPath, ProjectOptions options)
    {
        var slnContent = TemplateEngine.GenerateSolution(options);
        await File.WriteAllTextAsync(Path.Combine(projectPath, "src", $"{options.Name}.slnx"), slnContent);
    }

    private static async Task CreateApiProjectAsync(string projectPath, ProjectOptions options)
    {
        var apiPath = Path.Combine(projectPath, "src", $"{options.Name}.Api");

        // Create .csproj
        var csproj = TemplateEngine.GenerateApiCsproj(options);
        await File.WriteAllTextAsync(Path.Combine(apiPath, $"{options.Name}.Api.csproj"), csproj);

        // Create Program.cs
        var program = TemplateEngine.GenerateApiProgram(options);
        await File.WriteAllTextAsync(Path.Combine(apiPath, "Program.cs"), program);

        // Create appsettings.json
        var appsettings = TemplateEngine.GenerateAppSettings(options);
        await File.WriteAllTextAsync(Path.Combine(apiPath, "appsettings.json"), appsettings);

        // Create appsettings.Development.json
        var appsettingsDev = TemplateEngine.GenerateAppSettingsDevelopment();
        await File.WriteAllTextAsync(Path.Combine(apiPath, "appsettings.Development.json"), appsettingsDev);

        // Create Properties directory and launchSettings.json
        Directory.CreateDirectory(Path.Combine(apiPath, "Properties"));
        var launchSettings = TemplateEngine.GenerateApiLaunchSettings(options);
        await File.WriteAllTextAsync(Path.Combine(apiPath, "Properties", "launchSettings.json"), launchSettings);

        // Create Dockerfile
        var dockerfile = TemplateEngine.GenerateDockerfile(options);
        await File.WriteAllTextAsync(Path.Combine(apiPath, "Dockerfile"), dockerfile);
    }

    private static async Task CreateBlazorProjectAsync(string projectPath, ProjectOptions options)
    {
        var blazorPath = Path.Combine(projectPath, "src", $"{options.Name}.Blazor");

        // Create .csproj
        var csproj = TemplateEngine.GenerateBlazorCsproj();
        await File.WriteAllTextAsync(Path.Combine(blazorPath, $"{options.Name}.Blazor.csproj"), csproj);

        // Create Program.cs
        var program = TemplateEngine.GenerateBlazorProgram(options);
        await File.WriteAllTextAsync(Path.Combine(blazorPath, "Program.cs"), program);

        // Create _Imports.razor
        var imports = TemplateEngine.GenerateBlazorImports(options);
        await File.WriteAllTextAsync(Path.Combine(blazorPath, "_Imports.razor"), imports);

        // Create App.razor
        var app = TemplateEngine.GenerateBlazorApp();
        await File.WriteAllTextAsync(Path.Combine(blazorPath, "App.razor"), app);

        // Create wwwroot directory
        Directory.CreateDirectory(Path.Combine(blazorPath, "wwwroot"));

        // Create Shared directory and MainLayout.razor
        Directory.CreateDirectory(Path.Combine(blazorPath, "Shared"));
        var mainLayout = TemplateEngine.GenerateBlazorMainLayout(options);
        await File.WriteAllTextAsync(Path.Combine(blazorPath, "Shared", "MainLayout.razor"), mainLayout);

        // Create Pages directory
        Directory.CreateDirectory(Path.Combine(blazorPath, "Pages"));

        // Create Index.razor
        var index = TemplateEngine.GenerateBlazorIndexPage(options);
        await File.WriteAllTextAsync(Path.Combine(blazorPath, "Pages", "Index.razor"), index);
    }

    private static async Task CreateMigrationsProjectAsync(string projectPath, ProjectOptions options)
    {
        var migrationsPath = Path.Combine(projectPath, "src", $"{options.Name}.Migrations");

        // Create .csproj
        var csproj = TemplateEngine.GenerateMigrationsCsproj(options);
        await File.WriteAllTextAsync(Path.Combine(migrationsPath, $"{options.Name}.Migrations.csproj"), csproj);
    }

    private static async Task CreateAspireAppHostAsync(string projectPath, ProjectOptions options)
    {
        var appHostPath = Path.Combine(projectPath, "src", $"{options.Name}.AppHost");

        // Create .csproj
        var csproj = TemplateEngine.GenerateAppHostCsproj(options);
        await File.WriteAllTextAsync(Path.Combine(appHostPath, $"{options.Name}.AppHost.csproj"), csproj);

        // Create Program.cs
        var program = TemplateEngine.GenerateAppHostProgram(options);
        await File.WriteAllTextAsync(Path.Combine(appHostPath, "Program.cs"), program);

        // Create Properties directory and launchSettings.json
        Directory.CreateDirectory(Path.Combine(appHostPath, "Properties"));
        var launchSettings = TemplateEngine.GenerateAppHostLaunchSettings(options);
        await File.WriteAllTextAsync(Path.Combine(appHostPath, "Properties", "launchSettings.json"), launchSettings);
    }

    private static async Task CreateDockerComposeAsync(string projectPath, ProjectOptions options)
    {
        var dockerCompose = TemplateEngine.GenerateDockerCompose(options);
        await File.WriteAllTextAsync(Path.Combine(projectPath, "docker-compose.yml"), dockerCompose);

        var dockerComposeOverride = TemplateEngine.GenerateDockerComposeOverride();
        await File.WriteAllTextAsync(Path.Combine(projectPath, "docker-compose.override.yml"), dockerComposeOverride);
    }

    private static async Task CreateSampleModuleAsync(string projectPath, ProjectOptions options)
    {
        var modulePath = Path.Combine(projectPath, "src", "Modules", $"{options.Name}.Catalog");
        var contractsPath = Path.Combine(projectPath, "src", "Modules", $"{options.Name}.Catalog.Contracts");

        // Create Contracts project
        var contractsCsproj = TemplateEngine.GenerateCatalogContractsCsproj();
        await File.WriteAllTextAsync(Path.Combine(contractsPath, $"{options.Name}.Catalog.Contracts.csproj"), contractsCsproj);

        // Create Module project
        var moduleCsproj = TemplateEngine.GenerateCatalogModuleCsproj(options);
        await File.WriteAllTextAsync(Path.Combine(modulePath, $"{options.Name}.Catalog.csproj"), moduleCsproj);

        // Create CatalogModule.cs
        var catalogModule = TemplateEngine.GenerateCatalogModule(options);
        Directory.CreateDirectory(modulePath);
        await File.WriteAllTextAsync(Path.Combine(modulePath, "CatalogModule.cs"), catalogModule);

        // Create Features directory with sample endpoint
        var featuresPath = Path.Combine(modulePath, "Features", "v1", "Products");
        Directory.CreateDirectory(featuresPath);

        var getProducts = TemplateEngine.GenerateGetProductsEndpoint(options);
        await File.WriteAllTextAsync(Path.Combine(featuresPath, "GetProductsEndpoint.cs"), getProducts);
    }

    private static async Task CreateTerraformAsync(string projectPath, ProjectOptions options)
    {
        var terraformPath = Path.Combine(projectPath, "terraform");

        var mainTf = TemplateEngine.GenerateTerraformMain(options);
        await File.WriteAllTextAsync(Path.Combine(terraformPath, "main.tf"), mainTf);

        var variablesTf = TemplateEngine.GenerateTerraformVariables(options);
        await File.WriteAllTextAsync(Path.Combine(terraformPath, "variables.tf"), variablesTf);

        var outputsTf = TemplateEngine.GenerateTerraformOutputs(options);
        await File.WriteAllTextAsync(Path.Combine(terraformPath, "outputs.tf"), outputsTf);
    }

    private static async Task CreateGitHubActionsAsync(string projectPath, ProjectOptions options)
    {
        var workflowsPath = Path.Combine(projectPath, ".github", "workflows");

        var ciYaml = TemplateEngine.GenerateGitHubActionsCI(options);
        await File.WriteAllTextAsync(Path.Combine(workflowsPath, "ci.yml"), ciYaml);
    }

    private static async Task CreateCommonFilesAsync(string projectPath, ProjectOptions options)
    {
        // Create .gitignore
        var gitignore = TemplateEngine.GenerateGitignore();
        await File.WriteAllTextAsync(Path.Combine(projectPath, ".gitignore"), gitignore);

        // Create .editorconfig
        var editorconfig = TemplateEngine.GenerateEditorConfig();
        await File.WriteAllTextAsync(Path.Combine(projectPath, ".editorconfig"), editorconfig);

        // Create global.json
        var globalJson = TemplateEngine.GenerateGlobalJson();
        await File.WriteAllTextAsync(Path.Combine(projectPath, "global.json"), globalJson);

        // Create Directory.Build.props
        var buildProps = TemplateEngine.GenerateDirectoryBuildProps(options);
        await File.WriteAllTextAsync(Path.Combine(projectPath, "src", "Directory.Build.props"), buildProps);

        // Create Directory.Packages.props
        var packagesProps = TemplateEngine.GenerateDirectoryPackagesProps();
        await File.WriteAllTextAsync(Path.Combine(projectPath, "src", "Directory.Packages.props"), packagesProps);

        // Create README.md
        var readme = TemplateEngine.GenerateReadme(options);
        await File.WriteAllTextAsync(Path.Combine(projectPath, "README.md"), readme);
    }

    private static async Task RunDotnetRestoreAsync(string projectPath, ProjectOptions options)
    {
        AnsiConsole.MarkupLine("[grey]Running dotnet restore...[/]");

        var slnPath = Path.Combine(projectPath, $"{options.Name}.slnx");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore \"{slnPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = projectPath
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        if (process.ExitCode == 0)
        {
            ConsoleTheme.WriteSuccess("Dependencies restored successfully");
        }
        else
        {
            var error = await process.StandardError.ReadToEndAsync();
            ConsoleTheme.WriteWarning($"dotnet restore completed with warnings: {error}");
        }
    }

    private static void ShowNextSteps(ProjectOptions options)
    {
        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Rule("[green]Project created successfully![/]").RuleStyle(ConsoleTheme.PrimaryStyle));
        AnsiConsole.WriteLine();

        var panel = new Panel(new Markup($"""
            [bold]Next steps:[/]

            1. [grey]cd[/] [green]{options.Name}[/]
            {(options.IncludeAspire
                ? $"2. [grey]dotnet run --project[/] [green]src/{options.Name}.AppHost[/]"
                : $"2. [grey]dotnet run --project[/] [green]src/{options.Name}.Api[/]")}

            [bold]Useful commands:[/]

            [grey]dotnet build[/]          Build the solution
            [grey]dotnet test[/]           Run tests
            {(options.IncludeDocker ? "[grey]docker-compose up[/]    Start infrastructure" : "")}

            [bold]Documentation:[/]
            [link]https://fullstackhero.net[/]
            """))
            .Header("[bold] Getting Started [/]")
            .HeaderAlignment(Justify.Center)
            .BorderColor(ConsoleTheme.Primary)
            .Padding(2, 1);

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}
