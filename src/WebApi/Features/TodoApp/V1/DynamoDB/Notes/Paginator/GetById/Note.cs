namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

using Amazon.DynamoDBv2.Model;

public sealed class Note
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required Maybe<string> Content { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required DateTimeOffset ModifiedAt { get; init; }

    public required bool IsComplete { get; init; }

    public static Note From(Dictionary<string, AttributeValue> item)
    {
        var content = item.TryGetValue("content", out var attributeValue)
            ? Maybe.Some(attributeValue.S)
            : Maybe.None;
        return new Note
        {
            Id = Guid.Parse(item["id"].S),
            Title = item["title"].S,
            Content = content,
            CreatedAt = DateTimeOffset.Parse(item["created_at"].S),
            ModifiedAt = DateTimeOffset.Parse(item["modified_at"].S),
            IsComplete = item["is_complete"].BOOL,
        };
    }
}
