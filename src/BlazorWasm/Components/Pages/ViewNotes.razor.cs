namespace BlazorWasm.Components.Pages;

using BlazorWasm.Clients;
using Microsoft.AspNetCore.Components;
using MudBlazor;

public partial class ViewNotes
{
    private List<NoteDto> notes = new List<NoteDto>();

    private string token;

    private bool loading = true;

    [Inject]
    public INotesClient Client { get; set; }

    [Inject]
    public IDialogService Dialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await Refresh();
    }

    public async Task Refresh()
    {
        notes.Clear();
        var paginator = await Client.CreatePaginator(new CreatePaginatorRequest { MaxPageSize = 6 });

        var page = await Client.GetPage(paginator.Token);
        notes.AddRange(page.Notes);
        token = page.NextPageToken;
        loading = false;
    }

    public async Task LoadMore()
    {
        if (token is not null)
        {
            var page = await Client.GetPage(token);

            notes.AddRange(page.Notes);
            token = page.NextPageToken;
        }
    }

    public async Task OpenModal()
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true,
        };
        var dialog = await Dialog.ShowAsync<CreateNoteModal>("Create Note", options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Refresh();
        }
    }

    public async Task Complete(NoteDto note)
    {
        await Client.Complete(note.Id);
        note.IsComplete = true;
    }

    public async Task Delete(NoteDto note)
    {
        await Client.Delete(note.Id);
        notes.Remove(note);
    }
}
