namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

using Amazon.DynamoDBv2.Model;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : CommandHandler<Command, List<CommandResult>>
{
    protected override async ValueTask<List<CommandResult>> Handle(CancellationToken ct)
    {
        var result = new List<CommandResult>(Command.Requests.Count);
        var dynamoRequests = new List<WriteRequest>(Command.Requests.Count);
        var createdAt = dateTime.Now.ToString("O");

        foreach (var request in Command.Requests)
        {
            var id = Guid.NewGuid();
            var item = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new AttributeValue { S = $"USER#{Command.UserId}" },
                ["sk"] = new AttributeValue { S = $"NOTE#{id}" },
                ["type"] = new AttributeValue { S = "note_entity" },
                ["id"] = new AttributeValue { S = id.ToString() },
                ["title"] = new AttributeValue { S = request.Title },
                ["created_by"] = new AttributeValue { S = Command.UserId },
                ["created_at"] = new AttributeValue { S = createdAt },
                ["modified_at"] = new AttributeValue { S = createdAt },
                ["is_complete"] = new AttributeValue { BOOL = false },
            };
            if (request.Content.TryUnwrap(out var content))
            {
                item["content"] = new AttributeValue { S = content };
            }

            var commandResult = new CommandResult
            {
                Id = id,
                Title = request.Title,
            };
            result.Add(commandResult);
            dynamoRequests.Add(new WriteRequest { PutRequest = new PutRequest(item) });
        }

        foreach (var batch in dynamoRequests.Chunk(DynamoDb.MaxBatchSize))
        {
            var request = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>
                {
                    [db.Table] = batch.ToList(),
                },
            };

            await db.Client.BatchWriteItemAsync(request, ct);
        }

        return result;
    }
}
