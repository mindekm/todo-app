namespace WebApi.Middleware;

using System.Collections.Frozen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public sealed class FeatureFlagMiddleware(
    RequestDelegate next,
    IOptions<FeatureConfiguration> configuration,
    IProblemDetailsService service)
{
    private readonly FrozenDictionary<string, bool> configuration = configuration.Value.ToFrozenDictionary();

    public async Task InvokeAsync(HttpContext context)
    {
        var isEnabled = true;
        var flags = context.GetEndpoint()?.Metadata.GetOrderedMetadata<FeatureFlag>() ?? Array.Empty<FeatureFlag>();
        foreach (var flag in flags)
        {
            isEnabled &= configuration.TryGetValue(flag.Name, out var result) && result;
        }

        if (isEnabled)
        {
            await next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status404NotFound;
        var problemDetails = new ProblemDetails
        {
            Detail = "Endpoint is disabled",
            Status = StatusCodes.Status404NotFound,
        };
        await service.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problemDetails,
        });
    }
}
