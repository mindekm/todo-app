namespace WebApi.Features.Identity.V1.ApiKeys.Create;

public sealed class Command
{
    public required string Name { get; init; }

    public required List<string> Roles { get; init; }
}
