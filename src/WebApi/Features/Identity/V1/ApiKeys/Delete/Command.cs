namespace WebApi.Features.Identity.V1.ApiKeys.Delete;

public sealed class Command
{
    public required Guid Id { get; init; }
}
