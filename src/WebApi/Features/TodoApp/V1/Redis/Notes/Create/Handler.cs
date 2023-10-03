namespace WebApi.Features.TodoApp.V1.Redis.Notes.Create;

using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
using StackExchange.Redis;
using WebApi.Data.Redis;
using WebApi.FeatureHandlers;

public sealed class Handler(IConnectionMultiplexer multiplexer, IDateTime dateTime) : CommandHandler<Command, Guid>
{
    protected override async ValueTask<Guid> Handle(CancellationToken ct)
    {
        var createdAt = dateTime.Now;
        var entity = new NoteEntity
        {
            Id = Guid.NewGuid(),
            Title = Command.Title,
            Content = Command.Content.UnwrapOrDefault(),
            CreatedAt = createdAt,
            ModifiedAt = createdAt,
            IsComplete = false,
            CreatedBy = Command.UserId,
        };

        var db = multiplexer.GetDatabase();
        var key = $"User:{Command.UserId}__Note:{entity.Id}";

        using var buffer = new ArrayPoolBufferWriter<byte>();
        await using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, entity);
        await db.StringSetAsync(key, buffer.WrittenMemory);

        return entity.Id;
    }
}
