namespace FSH.Framework.Web.Cors;
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    public bool AllowAll { get; init; } = true;
    public string[] AllowedOrigins { get; init; } = [];
    public string[] AllowedHeaders { get; init; } = ["*"];
    public string[] AllowedMethods { get; init; } = ["*"];
}