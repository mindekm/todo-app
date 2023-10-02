namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.GetById;

using Amazon.DynamoDBv2.Model;
using Utilities;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db) : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
    {
        var request = new GetItemRequest();
        request.TableName = db.Table;
        request.Key = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"USER#{Query.UserId}" },
            ["sk"] = new AttributeValue { S = $"NOTE#{Query.Id}" },
        };

        var response = await db.Client.GetItemAsync(request, ct);
        if (!response.IsItemSet)
        {
            return Either.Left(DomainError.NotFound(Query.Id));
        }

        var content = response.Item.TryGetValue("content", out var attributeValue)
            ? Maybe.Some(attributeValue.S)
            : Maybe.None;
        var result = new QueryResult
        {
            Id = Guid.Parse(response.Item["id"].S),
            Title = response.Item["title"].S,
            Content = content,
            CreatedAt = DateTimeOffset.Parse(response.Item["created_at"].S),
            ModifiedAt = DateTimeOffset.Parse(response.Item["modified_at"].S),
            IsComplete = response.Item["is_complete"].BOOL,
        };
        return Either.Right(result);
    }
}
