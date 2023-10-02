namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

using Amazon.DynamoDBv2.Model;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : CommandHandler<Command, Guid>
{
    protected override async ValueTask<Guid> Handle(CancellationToken ct)
    {
        var request = new PutItemRequest { TableName = db.Table };

        var id = Guid.NewGuid();
        var createdAt = dateTime.Now.ToString("O");
        request.Item = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"USER#{Command.UserId}" },
            ["sk"] = new AttributeValue { S = $"NOTE#{id}" },
            ["type"] = new AttributeValue { S = "note_entity" },
            ["id"] = new AttributeValue { S = id.ToString() },
            ["title"] = new AttributeValue { S = Command.Title },
            ["created_by"] = new AttributeValue { S = Command.UserId },
            ["created_at"] = new AttributeValue { S = createdAt },
            ["modified_at"] = new AttributeValue { S = createdAt },
            ["is_complete"] = new AttributeValue { BOOL = false },
        };
        if (Command.Content.TryUnwrap(out var content))
        {
            request.Item["content"] = new AttributeValue { S = content };
        }

        await db.Client.PutItemAsync(request, ct);

        return id;
    }
}
