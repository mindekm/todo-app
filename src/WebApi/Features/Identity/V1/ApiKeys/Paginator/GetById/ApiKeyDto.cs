namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

public sealed class ApiKeyDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<string> Roles { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public static ApiKeyDto From(ApiKey key)
    {
        return new ApiKeyDto
        {
            Id = key.Id,
            Name = key.Name,
            Roles = key.Roles,
            CreatedAt = key.CreatedAt,
        };
    }
}
