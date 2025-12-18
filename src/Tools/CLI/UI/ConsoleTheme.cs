using Spectre.Console;

namespace FSH.CLI.UI;

internal static class ConsoleTheme
{
    // FullStackHero brand color
    public static Color Primary { get; } = new(62, 175, 124); // #3eaf7c
    public static Color Secondary { get; } = Color.White;
    public static Color Success { get; } = Color.Green;
    public static Color Warning { get; } = Color.Yellow;
    public static Color Error { get; } = Color.Red;
    public static Color Muted { get; } = Color.Grey;

    public static Style PrimaryStyle { get; } = new(Primary);
    public static Style SecondaryStyle { get; } = new(Secondary);
    public static Style SuccessStyle { get; } = new(Success);
    public static Style WarningStyle { get; } = new(Warning);
    public static Style ErrorStyle { get; } = new(Error);
    public static Style MutedStyle { get; } = new(Muted);

    public const string Banner = """

        ███████╗███████╗██╗  ██╗
        ██╔════╝██╔════╝██║  ██║
        █████╗  ███████╗███████║
        ██╔══╝  ╚════██║██╔══██║
        ██║     ███████║██║  ██║
        ╚═╝     ╚══════╝╚═╝  ╚═╝

        """;

    public const string Tagline = "FullStackHero .NET Starter Kit";

    public static void WriteBanner()
    {
        AnsiConsole.Write(new Text(Banner, PrimaryStyle));
        AnsiConsole.MarkupLine($"  [bold]{Tagline}[/]");
        AnsiConsole.WriteLine();
    }

    public static void WriteSuccess(string message) =>
        AnsiConsole.MarkupLine($"[green]✓[/] {message}");

    public static void WriteError(string message) =>
        AnsiConsole.MarkupLine($"[red]✗[/] {message}");

    public static void WriteWarning(string message) =>
        AnsiConsole.MarkupLine($"[yellow]![/] {message}");

    public static void WriteInfo(string message) =>
        AnsiConsole.MarkupLine($"[blue]i[/] {message}");

    public static void WriteStep(string message) =>
        AnsiConsole.MarkupLine($"[{Primary.ToMarkup()}]>[/] {message}");
}
