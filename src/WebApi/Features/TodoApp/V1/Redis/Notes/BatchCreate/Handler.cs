namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
using StackExchange.Redis;
using WebApi.Data.Redis;
using WebApi.FeatureHandlers;

public sealed class Handler(IConnectionMultiplexer multiplexer, IDateTime dateTime)
    : CommandHandler<Command, List<CommandResult>>
{
    protected override async ValueTask<List<CommandResult>> Handle(CancellationToken ct)
    {
        var result = new List<CommandResult>(Command.Requests.Count);
        var createdAt = dateTime.Now;

        var values = new KeyValuePair<RedisKey, RedisValue>[Command.Requests.Count];
        for (var i = 0; i < Command.Requests.Count; i++)
        {
            var request = Command.Requests[i];

            var id = Guid.NewGuid();
            var entity = new NoteEntity
            {
                Id = id,
                Title = request.Title,
                Content = request.Content.UnwrapOrDefault(),
                CreatedBy = Command.UserId,
                CreatedAt = createdAt,
                ModifiedAt = createdAt,
                IsComplete = false,
            };

            result.Add(new CommandResult
            {
                Id = entity.Id,
                Title = entity.Title,
            });

            using var buffer = new ArrayPoolBufferWriter<byte>();
            await using var writer = new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(writer, entity);

            var key = $"User:{Command.UserId}__Note:{id}";
            values[i] = new KeyValuePair<RedisKey, RedisValue>(key, buffer.WrittenMemory);
        }

        var db = multiplexer.GetDatabase();
        await db.StringSetAsync(values);

        return result;
    }
}
