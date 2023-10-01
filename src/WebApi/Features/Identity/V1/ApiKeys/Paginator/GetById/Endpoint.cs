namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public async ValueTask<Results<Ok<ResponseDto>, ValidationProblem, NotFound>> Action(
        [AsParameters] Parameters parameters,
        IValidator<Parameters> validator,
        Handler handler,
        CancellationToken ct)
    {
        var validationResult = validator.Validate(parameters);
        if (!validationResult.IsValid)
        {
            return ValidationProblem(validationResult);
        }

        var query = new Query { Token = parameters.Token };
        var resultOrError = await handler.Handle(query, ct);

        if (resultOrError.TryUnwrapRight(out var result))
        {
            var response = new ResponseDto
            {
                ApiKeys = result.ApiKeys.Select(ApiKeyDto.From).ToList(),
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
        builder.MapGet("api/identity/v1/api-keys/paginator", Action)
            .RequireAuthorization(Policies.CanManageApiKeys)
            .WithTags("ApiKeys")
            .WithGroupName(ApiGroup.Identity.V1)
            .WithFeatureFlags(FeatureFlag.DynamoDb);
    }
}
