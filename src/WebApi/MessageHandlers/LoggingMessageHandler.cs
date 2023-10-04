namespace WebApi.MessageHandlers;

public sealed partial class LoggingMessageHandler(ILogger<LoggingMessageHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var stopwatch = ValueStopwatch.StartNew();
        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            Log.RequestExecuted(
                logger,
                request.Method.ToString(),
                request.RequestUri?.ToString(),
                (int)response.StatusCode,
                stopwatch.GetElapsedTime().TotalMilliseconds);

            return response;
        }
        catch (HttpRequestException e)
        {
            Log.RequestExecutionFailed(
                logger,
                request.Method.ToString(),
                request.RequestUri?.ToString(),
                stopwatch.GetElapsedTime().TotalMilliseconds,
                e);

            throw;
        }
    }

    private static partial class Log
    {
        [LoggerMessage(LogLevel.Information, "Upstream {RequestMethod} {RequestUri} responded {StatusCode} in {Elapsed:0.0000} ms")]
        public static partial void RequestExecuted(
            ILogger logger,
            string requestMethod,
            string requestUri,
            int statusCode,
            double elapsed);

        [LoggerMessage(LogLevel.Error, "Upstream {RequestMethod} {RequestUri} failed in {Elapsed:0.0000} ms")]
        public static partial void RequestExecutionFailed(
            ILogger logger,
            string requestMethod,
            string requestUri,
            double elapsed,
            Exception exception);
    }
}
