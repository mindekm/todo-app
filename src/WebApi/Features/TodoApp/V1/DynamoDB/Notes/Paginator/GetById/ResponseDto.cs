namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

public sealed class ResponseDto
{
    public List<NoteDto> Notes { get; set; }

    public string NextPageToken { get; set; }
}
