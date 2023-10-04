namespace WebApi.Features.TodoApp.V1.Redis.Notes.Update;

using System.ComponentModel.DataAnnotations;

public sealed class RequestDto
{
    [StringLength(1000)]
    public string Content { get; set; }
}
