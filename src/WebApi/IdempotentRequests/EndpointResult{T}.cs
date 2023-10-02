namespace WebApi.IdempotentRequests;

public readonly record struct EndpointResult<T>(T Result, string Type)
{
    public bool IsCompatibleWith(string type)
        => string.Equals(Type, type, StringComparison.Ordinal);
}
