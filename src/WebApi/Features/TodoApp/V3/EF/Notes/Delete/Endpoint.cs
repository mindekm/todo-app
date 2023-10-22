namespace WebApi.Features.TodoApp.V3.EF.Notes.Delete;

using System.Security.Claims;
using WebApi.Authorization;

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
        var union = await handler.Handle(command, CancellationToken.None);

        return union.Match<Results<NoContent, NotFound>>(_ => NoContent(), error => NotFound());
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/todo-app/v3/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V3)
            .WithFeatureFlags(FeatureFlag.TodoAppV3, FeatureFlag.Ef);
    }
}
