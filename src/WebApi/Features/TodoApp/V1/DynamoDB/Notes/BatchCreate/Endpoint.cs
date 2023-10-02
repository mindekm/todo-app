namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<Ok<List<ResponseDto>>, ValidationProblem>> Action(
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
        return Ok(response);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/todo-app/v1/ddb/notes:batchCreate", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ddb")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.DynamoDb);
    }
}
