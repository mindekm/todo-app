namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.Create;

using System.ComponentModel;

public sealed class RequestDto
{
    [DefaultValue(50)]
    public int MaxPageSize { get; set; } = 50;

    public string SearchCondition { get; set; }
}
