namespace WebApi.Authorization;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null)
        {
            throw new InvalidOperationException("User id claim is missing");
        }

        return claim.Value;
    }
}
