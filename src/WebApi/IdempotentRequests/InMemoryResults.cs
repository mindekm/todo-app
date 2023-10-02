namespace WebApi.IdempotentRequests;

using Microsoft.Extensions.Caching.Memory;
using Utilities;

public sealed class InMemoryResults(IMemoryCache cache) : IIdempotentResults
{
    public ValueTask Store<T>(Guid key, EndpointResult<T> result, CancellationToken ct)
    {
        cache.Set(key, result, TimeSpan.FromMinutes(5));

        return ValueTask.CompletedTask;
    }

    public ValueTask<Maybe<EndpointResult<T>>> Retrieve<T>(Guid key, CancellationToken ct)
    {
        if (cache.TryGetValue<EndpointResult<T>>(key, out var result))
        {
            return ValueTask.FromResult(Maybe.Some(result));
        }

        Maybe<EndpointResult<T>> notFound = Maybe.None;
        return ValueTask.FromResult(notFound);
    }
}
