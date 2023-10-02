namespace WebApi.IdempotentRequests;

using Utilities;

public interface IIdempotentResults
{
    ValueTask Store<T>(Guid key, EndpointResult<T> result, CancellationToken ct);

    ValueTask<Maybe<EndpointResult<T>>> Retrieve<T>(Guid key, CancellationToken ct);
}
