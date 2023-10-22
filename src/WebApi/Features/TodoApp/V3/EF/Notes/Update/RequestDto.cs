namespace WebApi.Features.TodoApp.V3.EF.Notes.Update;

public sealed class RequestDto
{
    [StringLength(1000)]
    public string Content { get; set; }
}
