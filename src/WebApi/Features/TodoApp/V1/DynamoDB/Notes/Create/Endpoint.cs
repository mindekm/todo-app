namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<Ok<ResponseDto>, ValidationProblem>> Action(
        [FromBody] RequestDto request,
        IValidator<RequestDto> validator,
        ClaimsPrincipal principal,
        Handler handler,
        CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        var command = new Command
        {
            Title = request.Title,
            Content = request.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            UserId = principal.GetUserId(),
        };
        var result = await handler.Handle(command, CancellationToken.None);

        var response = new ResponseDto { Id = result };
        return Ok(response);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/todo-app/v1/ddb/notes", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ddb")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.DynamoDb);
    }
}
