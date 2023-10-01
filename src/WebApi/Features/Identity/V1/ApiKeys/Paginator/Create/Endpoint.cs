namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;

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

        var command = new Command { PageSize = request.MaxPageSize };
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
        builder.MapPost("api/identity/v1/api-keys/paginator", Action)
            .RequireAuthorization(Policies.CanManageApiKeys)
            .WithTags("ApiKeys")
            .WithGroupName(ApiGroup.Identity.V1)
            .WithFeatureFlags(FeatureFlag.DynamoDb);
    }
}
