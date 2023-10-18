namespace BlazorWasm.Components.Pages;

using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class CreateNoteModal
{
    private readonly CreateNoteModel model = new CreateNoteModel();

    [CascadingParameter]
    public MudDialogInstance Dialog { get; set; }

    [Inject]
    public INotesClient Client { get; set; }

    private async Task Submit()
    {
        var dto = new CreateNoteRequest
        {
            Title = model.Title,
            Content = model.Content,
            IdempotencyKey = Guid.NewGuid(),
        };
        await Client.Create(dto);
        Dialog.Close();
    }
}
