namespace WebApi.Features.Identity.V1.ApiKeys.Delete;

using Amazon.DynamoDBv2.Model;
using Utilities;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var metadataKey = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"APIKEY#{Command.Id}" },
            ["sk"] = new AttributeValue { S = $"APIKEY#{Command.Id}" },
        };
        var maybeKey = await GetKey(metadataKey, ct);
        if (!maybeKey.TryUnwrap(out var apiKey))
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        var entityKey = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"APIKEY#{apiKey}" },
            ["sk"] = new AttributeValue { S = $"APIKEY#{apiKey}" },
        };

        var request = new TransactWriteItemsRequest
        {
            TransactItems = new List<TransactWriteItem>
            {
                new TransactWriteItem
                {
                    Delete = new Delete
                    {
                        TableName = db.Table,
                        Key = metadataKey,
                    },
                },
                new TransactWriteItem
                {
                    Delete = new Delete
                    {
                        TableName = db.Table,
                        Key = entityKey,
                    },
                },
            },
        };

        await db.Client.TransactWriteItemsAsync(request, ct);
        return Maybe.None;
    }

    private async Task<Maybe<string>> GetKey(Dictionary<string, AttributeValue> entityKey, CancellationToken ct)
    {
        var request = new GetItemRequest();
        request.TableName = db.Table;
        request.Key = entityKey;

        var response = await db.Client.GetItemAsync(request, ct);
        if (!response.IsItemSet)
        {
            return Maybe.None;
        }

        return Maybe.Some(response.Item["key"].S);
    }
}
