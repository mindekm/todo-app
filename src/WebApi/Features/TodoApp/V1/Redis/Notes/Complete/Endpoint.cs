namespace WebApi.Features.TodoApp.V1.Redis.Notes.Complete;

using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<NoContent, NotFound>> Action(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        Handler handler)
    {
        var command = new Command
        {
            Id = id,
            UserId = principal.GetUserId(),
        };
        var error = await handler.Handle(command, CancellationToken.None);

        if (error.IsNone)
        {
            return NoContent();
        }

        return error.Unwrap() switch
        {
            ResourceNotFound => NotFound(),
            _ => throw new UnhandledDomainErrorException(),
        };
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/todo-app/v1/redis/notes/{id:guid}:complete", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Redis")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.Redis);
    }
}
