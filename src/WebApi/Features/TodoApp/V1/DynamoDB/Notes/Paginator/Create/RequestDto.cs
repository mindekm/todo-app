namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.Create;

using System.ComponentModel;

public sealed class RequestDto
{
    [DefaultValue(50)]
    public int MaxPageSize { get; set; } = 50;
}
