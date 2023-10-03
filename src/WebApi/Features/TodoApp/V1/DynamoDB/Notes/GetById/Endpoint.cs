namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.GetById;

using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<Ok<ResponseDto>, NotFound>> Action(
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

        if (errorOrResult.TryUnwrapRight(out var result))
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

        return errorOrResult.UnwrapLeft() switch
        {
            ResourceNotFound => NotFound(),
            _ => throw new UnhandledDomainErrorException(),
        };
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/todo-app/v1/ddb/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ddb")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.DynamoDb);
    }
}
