namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public async ValueTask<Results<Ok<ResponseDto>, ValidationProblem, NotFound>> Action(
        [AsParameters] Parameters parameters,
        IValidator<Parameters> validator,
        ClaimsPrincipal principal,
        Handler handler,
        CancellationToken ct)
    {
        var validationResult = validator.Validate(parameters);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        var query = new Query
        {
            Token = parameters.Token,
            UserId = principal.GetUserId(),
        };
        var resultOrError = await handler.Handle(query, ct);

        if (resultOrError.TryUnwrapRight(out var result))
        {
            var response = new ResponseDto
            {
                Notes = result.Notes.Select(NoteDto.From).ToList(),
                NextPageToken = result.NextPageToken.UnwrapOrDefault(),
            };
            return Ok(response);
        }

        return resultOrError.UnwrapLeft() switch
        {
            ResourceNotFound => NotFound(),
            _ => throw new UnhandledDomainErrorException(),
        };
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/todo-app/v1/ddb/notes/paginator", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ddb")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.DynamoDb);
    }
}
