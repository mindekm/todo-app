namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

using System.ComponentModel.DataAnnotations;

public sealed class CreateNoteDto
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(100)]
    public string Content { get; set; }
}
