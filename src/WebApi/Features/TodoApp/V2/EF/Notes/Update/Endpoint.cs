namespace WebApi.Features.TodoApp.V2.EF.Notes.Update;

using System.Security.Claims;
using FluentValidation;
using WebApi.Authorization;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<IResult> Action(
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
        var errorOrNone = await handler.Handle(command, CancellationToken.None);

        return errorOrNone.Match(MapError, NoContent);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPut("api/todo-app/v2/ef/notes/{id:guid}", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V2)
            .WithFeatureFlags(FeatureFlag.TodoAppV2, FeatureFlag.Ef)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
