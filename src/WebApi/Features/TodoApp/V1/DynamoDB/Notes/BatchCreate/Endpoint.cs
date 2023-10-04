namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.IdempotentRequests;

public sealed class Endpoint : EndpointBuilder
{
    private const string Route = "api/todo-app/v1/ddb/notes:batchCreate";

    public static async ValueTask<Results<Ok<List<ResponseDto>>, ValidationProblem, Conflict>> Action(
        [FromBody] RequestDto request,
        IValidator<RequestDto> validator,
        ClaimsPrincipal principal,
        Handler handler,
        IIdempotentResults results)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        if (request.TryGetIdempotencyKey().TryUnwrap(out var idempotencyKey))
        {
            var resultOrNothing = await results.Retrieve<List<ResponseDto>>(idempotencyKey, CancellationToken.None);
            if (resultOrNothing.TryUnwrap(out var cachedResult))
            {
                return cachedResult.IsCompatibleWith(Route) ? Ok(cachedResult.Result) : Conflict();
            }
        }

        var command = new Command
        {
            UserId = principal.GetUserId(),
            Requests = request.Requests
                .Select(r => new CreateNoteRequest
                {
                    Title = r.Title,
                    Content = r.Content.ToMaybe(NoneWhen.NullOrWhitespace),
                })
                .ToList(),
        };
        var result = await handler.Handle(command, CancellationToken.None);

        var response = result
            .Select(r => new ResponseDto
            {
                Id = r.Id,
                Title = r.Title,
            })
            .ToList();

        if (request.IsIdempotent())
        {
            await results.Store(idempotencyKey, EndpointResult.Create(response, Route), CancellationToken.None);
        }

        return Ok(response);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPost(Route, Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ddb")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.DynamoDb);
    }
}
