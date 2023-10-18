namespace BlazorWasm.Components.Layout;

using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class MainLayout
{
    private MudThemeProvider themeProvider;

    private bool isDrawerOpen = true;
    private bool isDarkMode = false;

    [Inject]
    public StateProvider StateProvider { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            isDarkMode = await themeProvider.GetSystemPreference();
            StateHasChanged();
        }
    }

    public void ToggleDrawer()
    {
        isDrawerOpen = !isDrawerOpen;
    }

    public void Logout()
    {
        StateProvider.Logout();
    }

    public void ToggleDarkMode()
    {
        isDarkMode = !isDarkMode;
    }
}
