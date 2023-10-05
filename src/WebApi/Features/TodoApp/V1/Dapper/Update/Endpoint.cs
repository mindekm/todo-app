namespace WebApi.Features.TodoApp.V1.Dapper.Update;

using System.Security.Claims;
using FluentValidation;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<NoContent, NotFound, ValidationProblem>> Action(
        [FromRoute] Guid id,
        [FromBody] RequestDto request,
        IValidator<RequestDto> validator,
        ClaimsPrincipal principal,
        Handler handler)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        var command = new Command
        {
            Id = id,
            Content = request.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            UserId = principal.GetUserId(),
        };
        var error = await handler.Handle(command, CancellationToken.None);

        if (error.IsNone)
        {
            return NoContent();
        }

        return error.Unwrap() switch
        {
            ResourceNotFound => NotFound(),
            _ => throw new InvalidOperationException(),
        };
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/todo-app/v1/dapper/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Dapper")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.Dapper);
    }
}
