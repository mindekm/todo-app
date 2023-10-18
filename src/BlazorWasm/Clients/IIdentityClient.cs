using Refit;

namespace BlazorWasm.Clients;

public interface IIdentityClient
{
    [Get("/api/identity/v1/current-claims")]
    Task<List<ClaimDto>> CurrentClaims([Header("Authorization")] string authorization);
}