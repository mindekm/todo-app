namespace BlazorWasm.Clients;

public sealed class GetPageResponse
{
    public List<NoteDto> Notes { get; set; }

    public string NextPageToken { get; set; }
}