namespace WebApi.MessageHandlers;

using Prometheus;

public sealed class InstrumentationMessageHandler : DelegatingHandler
{
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
        "upstream_request_duration_seconds",
        string.Empty,
        new HistogramConfiguration
        {
            Buckets = Histogram.ExponentialBuckets(0.001, 2, 16),
            LabelNames = new[] { "method", "code" },
        });

    private static readonly Counter RequestCounter = Metrics.CreateCounter(
        "upstream_requests_sent_total",
        string.Empty,
        new CounterConfiguration
        {
            LabelNames = new[] { "method", "code", },
        });

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = ValueStopwatch.StartNew();
        var response = await base.SendAsync(request, cancellationToken);
        var code = response.StatusCode.ToString("D");
        var method = request.Method.ToString();

        RequestDuration.WithLabels(method, code).Observe(stopwatch.GetElapsedTime().TotalSeconds);
        RequestCounter.WithLabels(method).Inc();

        return response;
    }
}
