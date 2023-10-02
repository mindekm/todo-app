namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

public sealed class RequestDto
{
    public string Title { get; set; }

    public string Content { get; set; }
}
