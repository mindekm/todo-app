namespace WebApi.IdempotentRequests;

public static class EndpointResult
{
    public static EndpointResult<T> Create<T>(T result, string type)
        => new EndpointResult<T>(result, type);
}
