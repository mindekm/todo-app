namespace WebApi.Features.Identity.V1.ApiKeys.Create;

public sealed class RequestDto
{
    [Required]
    public string Name { get; set; }

    [MinLength(1)]
    public List<string> Roles { get; set; } = new List<string>();
}
