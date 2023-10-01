namespace WebApi;

using Amazon.DynamoDBv2;
using Microsoft.Extensions.Options;

public sealed class DynamoDb(IAmazonDynamoDB client, IOptions<DynamoDbConfiguration> configuration)
{
    internal const int MaxBatchSize = 25;

    public IAmazonDynamoDB Client { get; } = client;

    public string Table => configuration.Value.Table;
}
