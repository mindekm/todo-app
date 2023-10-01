namespace WebApi.Middleware;

using Serilog.Context;
using Serilog.Core.Enrichers;

public sealed class TracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers.TryGetValue("x-request-id", out var values)
            ? values.ToString()
            : "N/A";

        context.TraceIdentifier = requestId;

        using (LogContext.Push(new PropertyEnricher("RequestId", requestId)))
        {
            await next(context);
        }
    }
}
