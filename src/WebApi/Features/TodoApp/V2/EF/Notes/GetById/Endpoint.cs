namespace WebApi.Features.TodoApp.V2.EF.Notes.GetById;

using System.Security.Claims;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<IResult> Action(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        Handler handler,
        CancellationToken ct)
    {
        var query = new Query
        {
            Id = id,
            UserId = principal.GetUserId(),
        };
        var errorOrResult = await handler.Handle(query, ct);

        return errorOrResult.Match(MapError, MapResult);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/todo-app/v2/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V2)
            .WithFeatureFlags(FeatureFlag.TodoAppV2, FeatureFlag.Ef)
            .Produces<ResponseDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static Ok<ResponseDto> MapResult(QueryResult result)
    {
        var response = new ResponseDto
        {
            Id = result.Id,
            Title = result.Title,
            Content = result.Content.UnwrapOrDefault(),
            CreatedAt = result.CreatedAt,
            ModifiedAt = result.ModifiedAt,
            IsComplete = result.IsComplete,
        };
        return Ok(response);
    }
}
