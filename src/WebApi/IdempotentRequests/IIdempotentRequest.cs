namespace WebApi.IdempotentRequests;

public interface IIdempotentRequest
{
    Guid? IdempotencyKey { get; }
}
