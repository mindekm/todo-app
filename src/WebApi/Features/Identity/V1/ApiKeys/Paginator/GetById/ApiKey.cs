namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

public sealed class ApiKey
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<string> Roles { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
}
