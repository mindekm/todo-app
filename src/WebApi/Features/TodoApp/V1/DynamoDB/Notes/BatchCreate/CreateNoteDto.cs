namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

public sealed class CreateNoteDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Content { get; set; }
}
