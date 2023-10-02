namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

public sealed class ResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }
}
