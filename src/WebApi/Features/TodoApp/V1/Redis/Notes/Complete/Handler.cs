namespace WebApi.Features.TodoApp.V1.Redis.Notes.Complete;

using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
using StackExchange.Redis;
using Utilities;
using WebApi.Data.Redis;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IConnectionMultiplexer multiplexer, IDateTime dateTime)
    : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var db = multiplexer.GetDatabase();
        var key = $"User:{Command.UserId}__Note:{Command.Id}";

        using var lease = await db.StringGetLeaseAsync(key);
        if (lease is null || lease.Length == 0)
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        var entity = JsonSerializer.Deserialize<NoteEntity>(lease.Span);

        entity.IsComplete = true;
        entity.ModifiedAt = dateTime.Now;

        using var buffer = new ArrayPoolBufferWriter<byte>();
        await using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, entity);

        await db.StringSetAsync(key, buffer.WrittenMemory);
        return Maybe.None;
    }
}
