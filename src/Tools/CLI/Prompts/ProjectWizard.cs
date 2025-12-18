using FSH.CLI.Models;
using FSH.CLI.UI;
using FSH.CLI.Validation;
using Spectre.Console;

namespace FSH.CLI.Prompts;

internal static class ProjectWizard
{
    public static ProjectOptions Run(string? initialName = null)
    {
        ConsoleTheme.WriteBanner();

        // Step 1: Choose preset or custom
        var startChoice = PromptStartChoice();

        if (startChoice != "Custom")
        {
            var preset = Presets.All.First(p => p.Name == startChoice);
            var presetName = PromptProjectName(initialName);
            var presetPath = PromptOutputPath();

            ShowSummary(preset.ToProjectOptions(presetName, presetPath));
            return preset.ToProjectOptions(presetName, presetPath);
        }

        // Custom flow
        var name = PromptProjectName(initialName);
        var type = PromptProjectType();
        var architecture = PromptArchitecture(type);
        var database = PromptDatabase(architecture);
        var features = PromptFeatures(architecture);
        var outputPath = PromptOutputPath();

        var options = new ProjectOptions
        {
            Name = name,
            Type = type,
            Architecture = architecture,
            Database = database,
            IncludeDocker = features.Contains("Docker Compose"),
            IncludeAspire = features.Contains("Aspire AppHost"),
            IncludeSampleModule = features.Contains("Sample Module (Todo)"),
            IncludeTerraform = features.Contains("Terraform (AWS)"),
            IncludeGitHubActions = features.Contains("GitHub Actions CI"),
            OutputPath = outputPath
        };

        ShowSummary(options);
        return options;
    }

    private static string PromptStartChoice()
    {
        var choices = new List<string> { "Custom" };
        choices.AddRange(Presets.All.Select(p => p.Name));

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("How would you like to start?")
                .PageSize(10)
                .HighlightStyle(ConsoleTheme.PrimaryStyle)
                .AddChoices(choices)
                .UseConverter(c =>
                {
                    if (c == "Custom")
                        return "[bold]Custom[/] - Choose your own options";

                    var preset = Presets.All.First(p => p.Name == c);
                    return $"[bold]{preset.Name}[/] - {preset.Description}";
                }));

        return choice;
    }

    private static string PromptProjectName(string? initialName)
    {
        if (!string.IsNullOrWhiteSpace(initialName) && OptionValidator.IsValidProjectName(initialName))
        {
            return initialName;
        }

        return AnsiConsole.Prompt(
            new TextPrompt<string>("Project [green]name[/]:")
                .PromptStyle(ConsoleTheme.PrimaryStyle)
                .ValidationErrorMessage("[red]Invalid project name[/]")
                .Validate(name =>
                {
                    if (string.IsNullOrWhiteSpace(name))
                        return Spectre.Console.ValidationResult.Error("Project name is required");

                    if (!char.IsLetter(name[0]))
                        return Spectre.Console.ValidationResult.Error("Project name must start with a letter");

                    if (!name.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.'))
                        return Spectre.Console.ValidationResult.Error("Project name can only contain letters, numbers, underscores, hyphens, or dots");

                    return Spectre.Console.ValidationResult.Success();
                }));
    }

    private static ProjectType PromptProjectType()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Project [green]type[/]:")
                .HighlightStyle(ConsoleTheme.PrimaryStyle)
                .AddChoices("API only", "API + Blazor (Full Stack)"));

        return choice == "API only" ? ProjectType.Api : ProjectType.ApiBlazor;
    }

    private static ArchitectureStyle PromptArchitecture(ProjectType projectType)
    {
        var choices = new List<string>
        {
            "Monolith (single deployable)",
            "Microservices (separate services)"
        };

        // Serverless not available with Blazor
        if (projectType == ProjectType.Api)
        {
            choices.Add("Serverless (AWS Lambda)");
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Architecture [green]style[/]:")
                .HighlightStyle(ConsoleTheme.PrimaryStyle)
                .AddChoices(choices));

        return choice switch
        {
            "Monolith (single deployable)" => ArchitectureStyle.Monolith,
            "Microservices (separate services)" => ArchitectureStyle.Microservices,
            "Serverless (AWS Lambda)" => ArchitectureStyle.Serverless,
            _ => ArchitectureStyle.Monolith
        };
    }

    private static DatabaseProvider PromptDatabase(ArchitectureStyle architecture)
    {
        var choices = new List<string>
        {
            "PostgreSQL",
            "SQL Server"
        };

        // SQLite not available with Microservices
        if (architecture != ArchitectureStyle.Microservices)
        {
            choices.Add("SQLite (dev only)");
        }

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Database [green]provider[/]:")
                .HighlightStyle(ConsoleTheme.PrimaryStyle)
                .AddChoices(choices));

        return choice switch
        {
            "PostgreSQL" => DatabaseProvider.PostgreSQL,
            "SQL Server" => DatabaseProvider.SqlServer,
            "SQLite (dev only)" => DatabaseProvider.SQLite,
            _ => DatabaseProvider.PostgreSQL
        };
    }

    private static List<string> PromptFeatures(ArchitectureStyle architecture)
    {
        var choices = new List<string>
        {
            "Docker Compose",
            "Sample Module (Todo)",
            "Terraform (AWS)",
            "GitHub Actions CI"
        };

        // Aspire not available with Serverless
        if (architecture != ArchitectureStyle.Serverless)
        {
            choices.Insert(1, "Aspire AppHost");
        }

        var defaults = new List<string> { "Docker Compose" };
        if (architecture != ArchitectureStyle.Serverless)
        {
            defaults.Add("Aspire AppHost");
        }

        var prompt = new MultiSelectionPrompt<string>()
            .Title("Additional [green]features[/]:")
            .HighlightStyle(ConsoleTheme.PrimaryStyle)
            .InstructionsText("[grey](Press [blue]<space>[/] to toggle, [green]<enter>[/] to accept)[/]")
            .AddChoices(choices);

        foreach (var item in defaults)
        {
            prompt.Select(item);
        }

        return AnsiConsole.Prompt(prompt);
    }

    private static string PromptOutputPath()
    {
        var useCurrentDir = AnsiConsole.Confirm("Create in [green]current directory[/]?", true);

        if (useCurrentDir)
        {
            return ".";
        }

        return AnsiConsole.Prompt(
            new TextPrompt<string>("Output [green]path[/]:")
                .PromptStyle(ConsoleTheme.PrimaryStyle)
                .DefaultValue(".")
                .ValidationErrorMessage("[red]Invalid path[/]")
                .Validate(path =>
                {
                    if (string.IsNullOrWhiteSpace(path))
                        return Spectre.Console.ValidationResult.Error("Path is required");

                    return Spectre.Console.ValidationResult.Success();
                }));
    }

    private static void ShowSummary(ProjectOptions options)
    {
        AnsiConsole.WriteLine();

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(ConsoleTheme.Primary)
            .AddColumn(new TableColumn("[bold]Option[/]").LeftAligned())
            .AddColumn(new TableColumn("[bold]Value[/]").LeftAligned());

        table.AddRow("Project Name", $"[green]{options.Name}[/]");
        table.AddRow("Project Type", FormatEnum(options.Type));
        table.AddRow("Architecture", FormatEnum(options.Architecture));
        table.AddRow("Database", FormatEnum(options.Database));
        table.AddRow("Docker Compose", FormatBool(options.IncludeDocker));
        table.AddRow("Aspire AppHost", FormatBool(options.IncludeAspire));
        table.AddRow("Sample Module", FormatBool(options.IncludeSampleModule));
        table.AddRow("Terraform (AWS)", FormatBool(options.IncludeTerraform));
        table.AddRow("GitHub Actions CI", FormatBool(options.IncludeGitHubActions));
        table.AddRow("Output Path", options.OutputPath);

        AnsiConsole.Write(new Panel(table)
            .Header("[bold] Project Configuration [/]")
            .HeaderAlignment(Justify.Center)
            .BorderColor(ConsoleTheme.Primary));

        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Proceed with this configuration?", true))
        {
            AnsiConsole.MarkupLine("[yellow]Aborted.[/]");
            Environment.Exit(0);
        }
    }

    private static string FormatEnum<T>(T value) where T : Enum =>
        value.ToString() switch
        {
            "Api" => "API only",
            "ApiBlazor" => "API + Blazor",
            "PostgreSQL" => "PostgreSQL",
            "SqlServer" => "SQL Server",
            "SQLite" => "SQLite",
            _ => value.ToString()
        };

    private static string FormatBool(bool value) =>
        value ? "[green]Yes[/]" : "[grey]No[/]";
}
