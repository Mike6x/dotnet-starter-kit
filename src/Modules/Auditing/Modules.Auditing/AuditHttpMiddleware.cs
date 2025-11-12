// Modules.Auditing/AuditHttpMiddleware.cs
using FSH.Modules.Auditing.Contracts;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace FSH.Modules.Auditing;

public sealed class AuditHttpMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuditHttpOptions _opts;
    private readonly IAuditPublisher _publisher;

    public AuditHttpMiddleware(RequestDelegate next, AuditHttpOptions opts, IAuditPublisher publisher)
        => (_next, _opts, _publisher) = (next, opts, publisher);

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (ShouldSkip(ctx))
        {
            await _next(ctx);
            return;
        }

        var sw = Stopwatch.StartNew();

        object? reqPreview = null;
        int reqSize = 0;
        if (_opts.CaptureBodies &&
            ContentTypeHelper.IsJsonLike(ctx.Request.ContentType, _opts.AllowedContentTypes))
        {
            (reqPreview, reqSize) = await HttpBodyReader.ReadRequestAsync(ctx, _opts.MaxRequestBytes, ctx.RequestAborted);
        }

        var originalBody = ctx.Response.Body;
        await using var tee = new MemoryStream();
        await using var respBuffer = new MemoryStream();
        ctx.Response.Body = tee;

        try
        {
            await _next(ctx);
            sw.Stop();

            object? respPreview = null;
            int respSize = 0;

            if (_opts.CaptureBodies &&
                ContentTypeHelper.IsJsonLike(ctx.Response.ContentType, _opts.AllowedContentTypes))
            {
                tee.Position = 0;
                await tee.CopyToAsync(respBuffer, ctx.RequestAborted);
                (respPreview, respSize) = await HttpBodyReader.ReadResponseAsync(
                    respBuffer, _opts.MaxResponseBytes, ctx.RequestAborted);
            }

            respBuffer.Position = 0;
            ctx.Response.Body = originalBody;
            if (respBuffer.Length > 0)
                await respBuffer.CopyToAsync(originalBody, ctx.RequestAborted);

            await Audit.ForActivity(Contracts.ActivityKind.Http, ctx.GetRoutePattern() ?? ctx.Request.Path)
                .WithActivityResult(
                    statusCode: ctx.Response.StatusCode,
                    durationMs: (int)sw.Elapsed.TotalMilliseconds,
                    captured: _opts.CaptureBodies ? BodyCapture.Both : BodyCapture.None,
                    requestSize: reqSize,
                    responseSize: respSize,
                    requestPreview: reqPreview,
                    responsePreview: respPreview)
                .WithSource("API")
                .WithTenant((_publisher.CurrentScope?.TenantId) ?? null)
                .WithUser(_publisher.CurrentScope?.UserId, _publisher.CurrentScope?.UserName)
                .WriteAsync(ctx.RequestAborted);
        }
        catch (Exception ex)
        {
            sw.Stop();

            var sev = ExceptionSeverityClassifier.Classify(ex);
            if (sev >= _opts.MinExceptionSeverity)
            {
                await Audit.ForException(ex, ExceptionArea.Api,
                        routeOrLocation: ctx.GetRoutePattern() ?? ctx.Request.Path, severity: sev)
                    .WithSource("API")
                    .WithTenant((_publisher.CurrentScope?.TenantId) ?? null)
                    .WithUser(_publisher.CurrentScope?.UserId, _publisher.CurrentScope?.UserName)
                    .WriteAsync(ctx.RequestAborted);
            }

            ctx.Response.Body = originalBody;
            throw;
        }
    }

    private bool ShouldSkip(HttpContext ctx)
    {
        var path = ctx.Request.Path.Value ?? string.Empty;
        return _opts.ExcludePathStartsWith.Any(prefix =>
            path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}
