namespace WebApi.Features.Identity.V1.CurrentClaims;

using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

public sealed class Endpoint : EndpointBuilder
{
    public static Ok<List<ClaimDto>> Action(ClaimsPrincipal principal)
    {
        var response = principal.Claims
            .Select(c => new ClaimDto
            {
                Type = c.Type,
                Value = c.Value,
            })
            .ToList();

        return Ok(response);
    }

    public override void Setup(IEndpointRouteBuilder builder)
    {
        builder.MapGet("api/identity/v1/current-claims", Action)
            .RequireAuthorization()
            .WithTags("Identity")
            .WithGroupName(ApiGroup.Identity.V1);
    }
}
