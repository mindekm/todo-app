namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.Create;

public sealed class ResponseDto
{
    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
