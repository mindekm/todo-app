namespace WebApi.Features.Identity.V1.ApiKeys.Delete;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.DomainErrors;

public sealed class Endpoint : EndpointBuilder
{
    public static async ValueTask<Results<NoContent, NotFound>> Action(
        [FromRoute] Guid id,
        Handler handler)
    {
        var command = new Command { Id = id };
        var result = await handler.Handle(command, CancellationToken.None);

        if (result.IsNone)
        {
            return NoContent();
        }

        return result.Unwrap() switch
        {
            ResourceNotFound => NotFound(),
            _ => throw new UnhandledDomainErrorException(),
        };
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("api/identity/v1/api-keys/{id:guid}", Action)
            .RequireAuthorization(Policies.CanManageApiKeys)
            .WithTags("ApiKeys")
            .WithGroupName(ApiGroup.Identity.V1)
            .WithFeatureFlags(FeatureFlag.DynamoDb);
    }
}
