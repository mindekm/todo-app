namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

public sealed class CommandResult
{
    public required string Token { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }
}
