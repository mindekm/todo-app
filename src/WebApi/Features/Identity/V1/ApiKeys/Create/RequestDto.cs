namespace WebApi.Features.Identity.V1.ApiKeys.Create;

public sealed class RequestDto
{
    public string Name { get; set; }

    public List<string> Roles { get; set; } = new List<string>();
}
