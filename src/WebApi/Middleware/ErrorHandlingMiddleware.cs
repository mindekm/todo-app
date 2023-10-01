namespace WebApi.Middleware;

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Serilog;

public sealed class ErrorHandlingMiddleware(RequestDelegate next, IProblemDetailsService service)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
        }
        catch (BadHttpRequestException e)
        {
            context.Response.StatusCode = e.StatusCode;
            var errors = new Dictionary<string, string[]>();
            if (e.InnerException is JsonException inner)
            {
                errors[inner.Path ?? "$"] = new[] { inner.Message };
            }
            else
            {
                errors["$"] = new[] { e.Message };
            }

            var details = new HttpValidationProblemDetails(errors);
            await service.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = details,
            });
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception has been thrown");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var details = new ProblemDetails
            {
                Detail = "Internal server error",
                Status = StatusCodes.Status500InternalServerError,
            };
            await service.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = details,
            });
        }
    }
}
