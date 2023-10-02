namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

public sealed class RequestDto
{
    public List<CreateNoteDto> Requests { get; set; } = new List<CreateNoteDto>();
}
