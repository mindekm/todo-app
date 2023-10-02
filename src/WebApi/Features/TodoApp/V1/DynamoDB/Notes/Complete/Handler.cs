namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Complete;

using Amazon.DynamoDBv2.Model;
using Utilities;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var request = new UpdateItemRequest();
        request.TableName = db.Table;
        request.Key = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"USER#{Command.UserId}" },
            ["sk"] = new AttributeValue { S = $"NOTE#{Command.Id}" },
        };
        request.UpdateExpression = "SET #isComplete = :isComplete, #modifiedAt = :modifiedAt";
        request.ConditionExpression = "attribute_exists(pk)";
        request.ExpressionAttributeNames = new Dictionary<string, string>
        {
            ["#isComplete"] = "is_complete",
            ["#modifiedAt"] = "modified_at",
        };
        request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            [":isComplete"] = new AttributeValue { BOOL = true },
            [":modifiedAt"] = new AttributeValue { S = dateTime.Now.ToString("O") },
        };

        try
        {
            await db.Client.UpdateItemAsync(request, ct);
        }
        catch (ConditionalCheckFailedException)
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        return Maybe.None;
    }
}
