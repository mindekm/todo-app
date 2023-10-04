namespace WebApi.IdempotentRequests;

using System.Text.Json;
using CommunityToolkit.HighPerformance.Buffers;
using StackExchange.Redis;

public sealed class RedisResults(IConnectionMultiplexer multiplexer) : IIdempotentResults
{
    public async ValueTask Store<T>(Guid key, EndpointResult<T> result, CancellationToken ct)
    {
        var database = multiplexer.GetDatabase();

        using var buffer = new ArrayPoolBufferWriter<byte>();
        await using var writer = new Utf8JsonWriter(buffer);
        JsonSerializer.Serialize(writer, ToDto(result));

        await database.StringSetAsync(FormatKey(key), buffer.WrittenMemory, TimeSpan.FromMinutes(5));
    }

    public async ValueTask<Maybe<EndpointResult<T>>> Retrieve<T>(Guid key, CancellationToken ct)
    {
        var database = multiplexer.GetDatabase();

        using var lease = await database.StringGetLeaseAsync(FormatKey(key));
        if (lease is null || lease.Length == 0)
        {
            return Maybe.None;
        }

        var dto = JsonSerializer.Deserialize<CacheItemDto<T>>(lease.Span, JsonSerializerOptions.Default);
        return Maybe.Some(EndpointResult.Create(dto.Item, dto.Type));
    }

    private static string FormatKey(Guid key) => $"EndpointResult__{key}";

    private static CacheItemDto<T> ToDto<T>(EndpointResult<T> result)
    {
        return new CacheItemDto<T>
        {
            Item = result.Result,
            Type = result.Type,
        };
    }

    private sealed class CacheItemDto<T>
    {
        public T Item { get; set; }

        public string Type { get; set; }
    }
}
