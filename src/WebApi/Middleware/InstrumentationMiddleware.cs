namespace WebApi.Middleware;

using Prometheus;

public sealed class InstrumentationMiddleware(RequestDelegate next)
{
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
        "downstream_request_duration_seconds",
        string.Empty,
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.001, 2, 16),
            LabelNames = new[]
            {
                "path",
                "code",
            },
        });

    private static readonly Counter RequestCount = Metrics.CreateCounter(
        "downstream_requests_received_total",
        string.Empty,
        new CounterConfiguration
        {
            LabelNames = new[]
            {
                "path",
                "code",
            },
        });

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = ValueStopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            var path = context.Request.Path;
            var code = context.Response.StatusCode.ToString();

            RequestDuration
                .WithLabels(path, code)
                .Observe(stopwatch.GetElapsedTime().TotalSeconds);

            RequestCount
                .WithLabels(path, code)
                .Inc();
        }
    }
}
