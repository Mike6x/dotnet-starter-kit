using System.ComponentModel.DataAnnotations;

namespace FSH.Framework.Web.Observability.OpenTelemetry;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetryOptions";

    /// <summary>
    /// Global switch to turn OpenTelemetry on/off.
    /// </summary>
    public bool Enabled { get; set; } = true;

    public TracingOptions Tracing { get; set; } = new();

    public MetricsOptions Metrics { get; set; } = new();

    public LoggingOptions Logging { get; set; } = new();

    public ExporterOptions Exporter { get; set; } = new();

    public SamplerOptions Sampler { get; set; } = new();

    public sealed class TracingOptions
    {
        public bool Enabled { get; set; } = true;
    }

    public sealed class MetricsOptions
    {
        public bool Enabled { get; set; } = true;
    }

    public sealed class LoggingOptions
    {
        public bool Enabled { get; set; } = true;

        public bool IncludeScopes { get; set; } = true;

        public bool IncludeFormattedMessage { get; set; } = true;
    }

    public sealed class ExporterOptions
    {
        public OtlpOptions Otlp { get; set; } = new();

        public JaegerExporterOptions Jaeger { get; set; } = new();
    }

    public sealed class OtlpOptions
    {
        public bool Enabled { get; set; } = true;

        [Url]
        public string? Endpoint { get; set; }

        /// <summary>
        /// Transport protocol, e.g. "grpc" or "http/protobuf".
        /// </summary>
        public string? Protocol { get; set; }
    }

    public sealed class JaegerExporterOptions
    {
        public bool Enabled { get; set; }

        /// <summary>
        /// Optional endpoint URI, e.g. http://jaeger:4317.
        /// If not set, Host/Port (agent) settings are used.
        /// </summary>
        [Url]
        public string? Endpoint { get; set; }

        public string? Host { get; set; }

        public int? Port { get; set; }
    }

    public sealed class SamplerOptions
    {
        /// <summary>
        /// Sampler type. Examples: "always_on", "always_off", "parentbased_traceidratio".
        /// </summary>
        public string Type { get; set; } = "parentbased_traceidratio";

        /// <summary>
        /// Sampling probability between 0.0 and 1.0 when using ratio-based sampling.
        /// </summary>
        [Range(0.0, 1.0)]
        public double Probability { get; set; } = 1.0;
    }
}
