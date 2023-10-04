namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.Create;

using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<Ok<ResponseDto>, ValidationProblem>> Action(
        [FromBody] RequestDto request,
        IValidator<RequestDto> validator,
        Handler handler)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        var command = new Command
        {
            PageSize = request.MaxPageSize,
            SearchCondition = request.SearchCondition.ToMaybe(NoneWhen.NullOrWhitespace),
        };
        var result = await handler.Handle(command, CancellationToken.None);

        var response = new ResponseDto
        {
            Token = result.Token,
            ExpiresAt = result.ExpiresAt,
        };
        return Ok(response);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapPost("api/todo-app/v1/ef/notes/paginator", Action)
            .RequireAuthorization()
            .WithTags("Notes", "Ef")
            .WithGroupName(ApiGroup.TodoApp.V1)
            .WithFeatureFlags(FeatureFlag.TodoAppV1, FeatureFlag.Ef);
    }
}
