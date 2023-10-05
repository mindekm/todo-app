namespace WebApi.Features.TodoApp.V1.Dapper.Update;

public sealed class RequestDto
{
    [StringLength(1000)]
    public string Content { get; set; }
}
