namespace WebApi.IdempotentRequests;

public static class IdempotentRequestExtensions
{
    public static bool IsIdempotent(this IIdempotentRequest request)
        => request.IdempotencyKey.HasValue;

    public static Maybe<Guid> TryGetIdempotencyKey(this IIdempotentRequest request)
        => request.IdempotencyKey.ToMaybe();
}
