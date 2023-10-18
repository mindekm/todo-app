namespace BlazorWasm.Clients;

public sealed class CreateNoteRequest
{
    public string Title { get; set; }

    public string Content { get; set; }

    public Guid? IdempotencyKey { get; set; }
}
