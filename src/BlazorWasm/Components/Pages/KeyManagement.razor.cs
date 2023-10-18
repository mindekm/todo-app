namespace BlazorWasm.Components.Pages;

using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class KeyManagement
{
    private readonly List<ApiKeyDto> keys = new List<ApiKeyDto>();

    [Inject]
    public IApiKeyClient Client { get; set; }

    [Inject]
    public IDialogService Dialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    public async Task LoadData()
    {
        keys.Clear();
        var paginator = await Client.CreatePaginator(new CreateApiKeyPaginatorRequest { MaxPageSize = 50 });
        var token = paginator.Token;

        if (token is null)
        {
            return;
        }

        while (token is not null)
        {
            var page = await Client.GetPage(token);
            keys.AddRange(page.ApiKeys);
            token = page.NextPageToken;
        }
    }

    public async Task Delete(ApiKeyDto dto)
    {
        await Client.Delete(dto.Id);
        keys.Remove(dto);
    }

    public async Task OpenModal()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            DisableBackdropClick = true,
        };
        var dialog = await Dialog.ShowAsync<CreateKeyModal>("Create API Key", options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await LoadData();
        }
    }
}
