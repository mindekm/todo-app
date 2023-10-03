namespace WebApi.Features.TodoApp.V1.Redis.Notes.GetById;

using System.Text.Json;
using StackExchange.Redis;
using Utilities;
using WebApi.Data.Redis;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IConnectionMultiplexer multiplexer) : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
    {
        var db = multiplexer.GetDatabase();
        var key = $"User:{Query.UserId}__Note:{Query.Id}";

        using var lease = await db.StringGetLeaseAsync(key);
        if (lease is null || lease.Length == 0)
        {
            return Either.Left(DomainError.NotFound(Query.Id));
        }

        var entity = JsonSerializer.Deserialize<NoteEntity>(lease.Span);

        var result = new QueryResult
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            CreatedAt = entity.CreatedAt,
            ModifiedAt = entity.ModifiedAt,
            IsComplete = entity.IsComplete,
        };
        return Either.Right(result);
    }
}
