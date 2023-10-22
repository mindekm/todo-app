namespace WebApi.Features.TodoApp.V2.EF.Notes.Delete;

using System.Security.Claims;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<IResult> Action(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        Handler handler)
    {
        var command = new Command
        {
            Id = id,
            UserId = principal.GetUserId(),
        };
        var errorOrNone = await handler.Handle(command, CancellationToken.None);

        return errorOrNone.Match(MapError, NoContent);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/todo-app/v2/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V2)
            .WithFeatureFlags(FeatureFlag.TodoAppV2, FeatureFlag.Ef)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
