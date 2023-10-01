namespace WebApi;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

public sealed class DynamoDbSeeder(IServiceProvider provider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = provider.CreateScope();
        var dynamoDb = scope.ServiceProvider.GetRequiredService<DynamoDb>();

        var tables = await dynamoDb.Client.ListTablesAsync(stoppingToken);
        if (tables.TableNames.Count == 0)
        {
            var request = new CreateTableRequest
            {
                TableName = dynamoDb.Table,
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement("pk", KeyType.HASH),
                    new KeySchemaElement("sk", KeyType.RANGE),
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition("pk", ScalarAttributeType.S),
                    new AttributeDefinition("sk", ScalarAttributeType.S),
                    new AttributeDefinition("gsi1pk", ScalarAttributeType.S),
                    new AttributeDefinition("gsi1sk", ScalarAttributeType.S),
                },
                BillingMode = BillingMode.PAY_PER_REQUEST,
                ProvisionedThroughput = new ProvisionedThroughput(5, 5),
                GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
                {
                    new GlobalSecondaryIndex
                    {
                        Projection = new Projection { ProjectionType = ProjectionType.ALL },
                        IndexName = "gsi1",
                        KeySchema = new List<KeySchemaElement>
                        {
                            new KeySchemaElement("gsi1pk", KeyType.HASH),
                            new KeySchemaElement("gsi1sk", KeyType.RANGE),
                        },
                    },
                },
            };

            await dynamoDb.Client.CreateTableAsync(request, stoppingToken);

            await dynamoDb.Client.UpdateTimeToLiveAsync(
                new UpdateTimeToLiveRequest
                {
                    TableName = dynamoDb.Table,
                    TimeToLiveSpecification = new TimeToLiveSpecification
                    {
                        Enabled = true,
                        AttributeName = "ttl",
                    },
                },
                stoppingToken);

            await CreateDevelopmentKey(dynamoDb, stoppingToken);
        }
    }

    private static async Task CreateDevelopmentKey(DynamoDb db, CancellationToken ct)
    {
        var id = Guid.NewGuid().ToString();

        var keyItem = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = "APIKEY#development" },
            ["sk"] = new AttributeValue { S = "APIKEY#development" },
            ["type"] = new AttributeValue { S = "apikey_entity" },
            ["id"] = new AttributeValue { S = id },
            ["name"] = new AttributeValue { S = "Development key" },
            ["roles"] = new AttributeValue { SS = new List<string> { "Admin" } },
        };

        var metadataItem = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"APIKEY#{id}" },
            ["sk"] = new AttributeValue { S = $"APIKEY#{id}" },
            ["gsi1pk"] = new AttributeValue { S = "APIKEYS" },
            ["gsi1sk"] = new AttributeValue { S = "APIKEY#development key" },
            ["type"] = new AttributeValue { S = "apikey_metadata" },
            ["id"] = new AttributeValue { S = id },
            ["name"] = new AttributeValue { S = "Development key" },
            ["key"] = new AttributeValue { S = "development" },
            ["created_at"] = new AttributeValue { S = DateTime.Now.ToString("O") },
            ["roles"] = new AttributeValue { SS = new List<string> { "Admin" } },
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
    }
}
