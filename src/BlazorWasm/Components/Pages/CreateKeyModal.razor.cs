namespace BlazorWasm.Components.Pages;

using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class CreateKeyModal
{
    private readonly string[] roles = ["Admin", "User", "Role1", "Role2"];

    private readonly CreateKeyModel model = new CreateKeyModel();

    private bool submitted;

    private string key;

    [CascadingParameter]
    public MudDialogInstance Dialog { get; set; }

    [Inject]
    public IApiKeyClient Client { get; set; }

    private void Cancel()
    {
        Dialog.Cancel();
    }

    private async Task Submit()
    {
        var dto = new CreateApiKeyRequest
        {
            Name = model.Name,
            Roles = model.Roles.ToList(),
        };
        var response = await Client.Create(dto);
        submitted = true;
        key = response.Key;
        StateHasChanged();
    }
}
