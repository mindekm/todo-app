namespace WebApi.Features.Identity.V1.ApiKeys.Create;

public sealed class CommandResult
{
    public required Guid Id { get; init; }

    public required string Key { get; init; }
}
