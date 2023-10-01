namespace WebApi;

public sealed class UtcDateTime : IDateTime
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;
}
