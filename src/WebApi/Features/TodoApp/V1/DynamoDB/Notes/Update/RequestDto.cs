namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Update;

public sealed class RequestDto
{
    [StringLength(1000)]
    public string Content { get; set; }
}
