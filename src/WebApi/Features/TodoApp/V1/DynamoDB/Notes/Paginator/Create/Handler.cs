namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.Create;

using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : CommandHandler<Command, CommandResult>
{
    protected override async ValueTask<CommandResult> Handle(CancellationToken ct)
    {
        var token = Guid.NewGuid().ToString();
        var expiration = dateTime.Now.AddMinutes(5);
        var dto = new PageDto { PageSize = Command.PageSize };

        var request = new PutItemRequest();
        request.TableName = db.Table;
        request.Item = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"PAGINATOR#{token}" },
            ["sk"] = new AttributeValue { S = $"PAGINATOR#{token}" },
            ["type"] = new AttributeValue { S = "paginator" },
            ["data"] = new AttributeValue { S = JsonSerializer.Serialize(dto) },
            ["ttl"] = new AttributeValue { N = expiration.ToUnixTimeSeconds().ToString() },
        };

        await db.Client.PutItemAsync(request, ct);

        return new CommandResult
        {
            Token = token,
            ExpiresAt = expiration,
        };
    }
}
