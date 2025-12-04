using Hangfire.Common;
using Hangfire.Server;
using System.Diagnostics;

namespace FSH.Framework.Jobs;

/// <summary>
/// Adds basic tracing around Hangfire job execution.
/// </summary>
public sealed class HangfireTelemetryFilter : JobFilterAttribute, IServerFilter
{
    private const string ActivityKey = "__fsh_activity";
    private static readonly ActivitySource ActivitySource = new("FSH.Hangfire");

    public void OnPerforming(PerformingContext filterContext)
    {
        ArgumentNullException.ThrowIfNull(filterContext);

        var job = filterContext.BackgroundJob?.Job;
        string name = job is null
            ? "Hangfire.Job"
            : $"{job.Type.Name}.{job.Method.Name}";

        var activity = ActivitySource.StartActivity(name, ActivityKind.Internal);
        if (activity is null)
        {
            return;
        }

        activity.SetTag("hangfire.job_id", filterContext.BackgroundJob?.Id);
        activity.SetTag("hangfire.job_type", job?.Type.FullName);
        activity.SetTag("hangfire.job_method", job?.Method.Name);

        filterContext.Items[ActivityKey] = activity;
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        ArgumentNullException.ThrowIfNull(filterContext);

        if (!filterContext.Items.TryGetValue(ActivityKey, out var value) || value is not Activity activity)
        {
            return;
        }

        if (filterContext.Exception is not null)
        {
            activity.SetStatus(ActivityStatusCode.Error);
            activity.SetTag("exception.type", filterContext.Exception.GetType().FullName);
            activity.SetTag("exception.message", filterContext.Exception.Message);
        }
        else
        {
            activity.SetStatus(ActivityStatusCode.Ok);
        }

        activity.Dispose();
    }
}
