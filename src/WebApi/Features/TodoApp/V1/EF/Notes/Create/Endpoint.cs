namespace WebApi.Features.TodoApp.V1.EF.Notes.Create;

using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.IdempotentRequests;

public sealed class Endpoint : EndpointBuilder
{
    private const string Route = "api/todo-app/v1/ef/notes";

    public static async ValueTask<Results<Ok<ResponseDto>, ValidationProblem, Conflict>> Action(
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
            var maybeCachedResult = await results.Retrieve<ResponseDto>(idempotencyKey, CancellationToken.None);
            if (maybeCachedResult.TryUnwrap(out var cachedResult))
            {
                return cachedResult.IsCompatibleWith(Route) ? Ok(cachedResult.Result) : Conflict();
            }
        }

        var command = new Command
        {
            Title = request.Title,
            Content = request.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            UserId = principal.GetUserId(),
        };
        var result = await handler.Handle(command, CancellationToken.None);

        var response = new ResponseDto { Id = result };

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
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.Ef);
    }
}
