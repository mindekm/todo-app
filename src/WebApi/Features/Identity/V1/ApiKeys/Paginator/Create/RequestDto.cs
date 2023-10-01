namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

using System.ComponentModel;

public sealed class RequestDto
{
    [DefaultValue(50)]
    public int MaxPageSize { get; set; } = 50;
}
