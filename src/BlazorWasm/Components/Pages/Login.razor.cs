namespace BlazorWasm.Components.Pages;

using System.Net;
using System.Security.Claims;
using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;

public partial class Login
{
    private bool isPasswordVisible;
    private InputType passwordInputType = InputType.Password;
    private string passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private string key;

    [Inject]
    public StateProvider StateProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public IIdentityClient Client { get; set; }

    [Inject]
    public ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var state = await StateProvider.GetAuthenticationStateAsync();
        if (state.User.Identity?.IsAuthenticated ?? false)
        {
            NavigationManager.NavigateTo("/");
        }
    }

    public async Task Submit()
    {
        try
        {
            var dtos = await Client.CurrentClaims($"ApiKey {key ?? string.Empty}");
            var claims = dtos.Select(d => new Claim(d.Type, d.Value)).ToList();
            claims.Add(new Claim("Key", key));

            StateProvider.Login(claims);

            key = null;
            NavigationManager.NavigateTo("/");
        }
        catch (ValidationApiException e)
        {
            if (e.StatusCode == HttpStatusCode.Unauthorized)
            {
                Snackbar.Add("Invalid key entered", Severity.Error);
            }
            else
            {
                Snackbar.Add("Failed to authorize", Severity.Error);
            }
        }
    }

    public void UseDevelopmentKey()
    {
        key = "development";
    }

    public void TogglePasswordVisibility()
    {
        if (isPasswordVisible)
        {
            isPasswordVisible = false;
            passwordInputType = InputType.Password;
            passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            isPasswordVisible = true;
            passwordInputType = InputType.Text;
            passwordInputIcon = Icons.Material.Filled.Visibility;
        }
    }
}
