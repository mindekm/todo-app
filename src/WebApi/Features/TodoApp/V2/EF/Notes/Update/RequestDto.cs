namespace WebApi.Features.TodoApp.V2.EF.Notes.Update;

public sealed class RequestDto
{
    [StringLength(1000)]
    public string Content { get; set; }
}
