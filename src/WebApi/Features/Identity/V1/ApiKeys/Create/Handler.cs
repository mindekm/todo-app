namespace WebApi.Features.Identity.V1.ApiKeys.Create;

using System.Security.Cryptography;
using Amazon.DynamoDBv2.Model;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : CommandHandler<Command, CommandResult>
{
    protected override async ValueTask<CommandResult> Handle(CancellationToken ct)
    {
        var key = CreateKey();
        var id = Guid.NewGuid();

        var keyItem = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"APIKEY#{key}" },
            ["sk"] = new AttributeValue { S = $"APIKEY#{key}" },
            ["type"] = new AttributeValue { S = "apikey_entity" },
            ["id"] = new AttributeValue { S = id.ToString() },
            ["name"] = new AttributeValue { S = Command.Name },
            ["roles"] = new AttributeValue { SS = Command.Roles },
        };

        var metadataItem = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"APIKEY#{id}" },
            ["sk"] = new AttributeValue { S = $"APIKEY#{id}" },
            ["gsi1pk"] = new AttributeValue { S = "APIKEYS" },
            ["gsi1sk"] = new AttributeValue { S = $"APIKEY#{Command.Name.ToLower()}" },
            ["type"] = new AttributeValue { S = "apikey_metadata" },
            ["id"] = new AttributeValue { S = id.ToString() },
            ["name"] = new AttributeValue { S = Command.Name },
            ["key"] = new AttributeValue { S = key },
            ["created_at"] = new AttributeValue { S = dateTime.Now.ToString("O") },
            ["roles"] = new AttributeValue { SS = Command.Roles },
        };

        var request = new TransactWriteItemsRequest
        {
            TransactItems = new List<TransactWriteItem>
            {
                new TransactWriteItem
                {
                    Put = new Put
                    {
                        Item = keyItem,
                        TableName = db.Table,
                        ConditionExpression = "attribute_not_exists(pk)",
                    },
                },
                new TransactWriteItem
                {
                    Put = new Put
                    {
                        Item = metadataItem,
                        TableName = db.Table,
                        ConditionExpression = "attribute_not_exists(pk)",
                    },
                },
            },
        };

        await db.Client.TransactWriteItemsAsync(request, ct);

        return new CommandResult
        {
            Id = id,
            Key = key,
        };
    }

    private static string CreateKey()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToHexString(bytes);
    }
}
