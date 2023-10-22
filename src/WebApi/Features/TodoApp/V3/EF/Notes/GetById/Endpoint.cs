namespace WebApi.Features.TodoApp.V3.EF.Notes.GetById;

using System.Security.Claims;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<Ok<ResponseDto>, NotFound, BadRequest>> Action(
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
        var union = await handler.Handle(query, ct);

        return union.Match<Results<Ok<ResponseDto>, NotFound, BadRequest>>(
            result => MapResult(result),
            notFoundError => NotFound(),
            invalidOperationError => BadRequest());
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/todo-app/v3/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V3)
            .WithFeatureFlags(FeatureFlag.TodoAppV3, FeatureFlag.Ef);
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
