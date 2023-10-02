namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Delete;

using Amazon.DynamoDBv2.Model;
using Utilities;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var request = new DeleteItemRequest();
        request.TableName = db.Table;
        request.Key = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"USER#{Command.UserId}" },
            ["sk"] = new AttributeValue { S = $"NOTE#{Command.Id}" },
        };
        request.ConditionExpression = "attribute_exists(pk)";

        try
        {
            await db.Client.DeleteItemAsync(request, ct);
        }
        catch (ConditionalCheckFailedException)
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        return Maybe.None;
    }
}
