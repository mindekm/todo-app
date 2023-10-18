namespace BlazorWasm;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

public sealed class StateProvider : AuthenticationStateProvider
{
    public ClaimsPrincipal CurrentUser { get; private set; } = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(CurrentUser));
    }

    public void Login(IEnumerable<Claim> claims)
    {
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "blazor"));
        foreach (var claim in CurrentUser.Claims)
        {
            Console.WriteLine(claim.Type);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CurrentUser)));
    }

    public void Logout()
    {
        CurrentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CurrentUser)));
    }
}
