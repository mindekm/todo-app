namespace WebApi.Features.TodoApp.V3.EF.Notes.Update;

using System.Security.Claims;
using FluentValidation;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<NoContent, NotFound, ValidationProblem, BadRequest>> Action(
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
        var union = await handler.Handle(command, CancellationToken.None);

        return union.Match<Results<NoContent, NotFound, ValidationProblem, BadRequest>>(
            _ => NoContent(),
            notFoundError => NotFound(),
            invalidOperationError => BadRequest());
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/todo-app/v3/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V3)
            .WithFeatureFlags(FeatureFlag.TodoAppV3, FeatureFlag.Ef);
    }
}
