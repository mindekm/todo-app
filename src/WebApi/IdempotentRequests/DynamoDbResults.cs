namespace WebApi.IdempotentRequests;

using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using Utilities;

public sealed class DynamoDbResults(DynamoDb db, IDateTime dateTime) : IIdempotentResults
{
    public async ValueTask Store<T>(Guid key, EndpointResult<T> result, CancellationToken ct)
    {
        var expiration = dateTime.Now.AddMinutes(5).ToUnixTimeSeconds().ToString();

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, result.Result, JsonSerializerOptions.Default, ct);

        var request = new PutItemRequest();
        request.TableName = db.Table;
        request.Item = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = FormatKey(key) },
            ["sk"] = new AttributeValue { S = FormatKey(key) },
            ["type"] = new AttributeValue { S = "endpoint_result" },
            ["endpoint"] = new AttributeValue { S = result.Type },
            ["data"] = new AttributeValue { B = stream },
            ["ttl"] = new AttributeValue { N = expiration },
        };

        await db.Client.PutItemAsync(request, ct);
    }

    public async ValueTask<Maybe<EndpointResult<T>>> Retrieve<T>(Guid key, CancellationToken ct)
    {
        var request = new GetItemRequest();
        request.TableName = db.Table;
        request.Key = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = FormatKey(key) },
            ["sk"] = new AttributeValue { S = FormatKey(key) },
        };

        var response = await db.Client.GetItemAsync(request, ct);
        if (!response.IsItemSet)
        {
            return Maybe.None;
        }

        var endpoint = response.Item["endpoint"].S;
        using var stream = response.Item["data"].B;
        var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions.Default, ct);
        return result is null
            ? Maybe.None
            : Maybe.Some(EndpointResult.Create(result, endpoint));
    }

    private static string FormatKey(Guid key) => $"ENDPOINTRESULT#{key}";
}
