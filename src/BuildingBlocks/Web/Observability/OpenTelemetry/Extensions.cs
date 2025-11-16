using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using static FSH.Framework.Web.Observability.OpenTelemetry.OpenTelemetryOptions;

namespace FSH.Framework.Web.Observability.OpenTelemetry;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    private const string TenantIdTagName = "fsh.tenant_id";
    private const string UserIdTagName = "enduser.id";
    private const string CorrelationIdTagName = "fsh.correlation_id";

    public static IHostApplicationBuilder AddHeroOpenTelemetry(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var options = new OpenTelemetryOptions();
        builder.Configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(options);

        if (!options.Enabled)
        {
            return builder;
        }

        builder.Services.AddOptions<OpenTelemetryOptions>()
            .BindConfiguration(OpenTelemetryOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(serviceName: builder.Environment.ApplicationName);

        ConfigureLogging(builder, options);
        ConfigureMetricsAndTracing(builder, options, resourceBuilder);

        return builder;
    }

    private static void ConfigureLogging(
        IHostApplicationBuilder builder,
        OpenTelemetryOptions options)
    {
        if (!options.Logging.Enabled)
        {
            return;
        }

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = options.Logging.IncludeScopes;
            logging.IncludeFormattedMessage = options.Logging.IncludeFormattedMessage;
            logging.ParseStateValues = true;

            if (options.Exporter.Otlp.Enabled)
            {
                logging.AddOtlpExporter(otlp =>
                {
                    ConfigureOtlpExporter(options.Exporter.Otlp, otlp);
                });
            }
        });
    }

    private static void ConfigureMetricsAndTracing(
        IHostApplicationBuilder builder,
        OpenTelemetryOptions options,
        ResourceBuilder resourceBuilder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(rb => rb.AddService(builder.Environment.ApplicationName))
            .WithMetrics(metrics =>
            {
                if (!options.Metrics.Enabled)
                {
                    return;
                }

                metrics
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                if (options.Exporter.Otlp.Enabled)
                {
                    metrics.AddOtlpExporter(otlp =>
                    {
                        ConfigureOtlpExporter(options.Exporter.Otlp, otlp);
                    });
                }
            })
            .WithTracing(tracing =>
            {
                if (!options.Tracing.Enabled)
                {
                    return;
                }

                tracing
                    .SetResourceBuilder(resourceBuilder)
                    .SetSampler(CreateSampler(options.Sampler))
                    .AddAspNetCoreInstrumentation(instrumentation =>
                    {
                        instrumentation.Filter = context => !IsHealthCheck(context.Request.Path);
                        instrumentation.EnrichWithHttpRequest = EnrichWithHttpRequest;
                        instrumentation.EnrichWithHttpResponse = EnrichWithHttpResponse;
                    })
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddSource(builder.Environment.ApplicationName);

                if (options.Exporter.Otlp.Enabled)
                {
                    tracing.AddOtlpExporter(otlp =>
                    {
                        ConfigureOtlpExporter(options.Exporter.Otlp, otlp);
                    });
                }
            });
    }

    private static bool IsHealthCheck(PathString path) =>
        path.StartsWithSegments(HealthEndpointPath) ||
        path.StartsWithSegments(AlivenessEndpointPath);

    private static void EnrichWithHttpRequest(Activity activity, HttpRequest request)
    {
        activity.SetTag("http.method", request.Method);
        activity.SetTag("http.scheme", request.Scheme);
        activity.SetTag("http.host", request.Host.Value);
        activity.SetTag("http.target", request.Path);
    }

    private static void EnrichWithHttpResponse(Activity activity, HttpResponse response)
    {
        activity.SetTag("http.status_code", response.StatusCode);
    }

    private static Sampler CreateSampler(OpenTelemetryOptions.SamplerOptions options)
    {
        var type = options.Type?.Trim().ToLowerInvariant();

        return type switch
        {
            "always_off" => new AlwaysOffSampler(),
            "always_on" => new AlwaysOnSampler(),
            "traceidratio" => new TraceIdRatioBasedSampler(options.Probability),
            "parentbased_always_on" => new ParentBasedSampler(new AlwaysOnSampler()),
            "parentbased_traceidratio" or _ => new ParentBasedSampler(new TraceIdRatioBasedSampler(options.Probability)),
        };
    }

    private static void ConfigureOtlpExporter(
        OtlpOptions options,
        OtlpExporterOptions otlp)
    {
        if (!string.IsNullOrWhiteSpace(options.Endpoint))
        {
            otlp.Endpoint = new Uri(options.Endpoint);
        }

        var protocol = options.Protocol?.Trim().ToLowerInvariant();
        otlp.Protocol = protocol switch
        {
            "grpc" => OtlpExportProtocol.Grpc,
            "http/protobuf" => OtlpExportProtocol.HttpProtobuf,
            _ => otlp.Protocol
        };
    }
}
