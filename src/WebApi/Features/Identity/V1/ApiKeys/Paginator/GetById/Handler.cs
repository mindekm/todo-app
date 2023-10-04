namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(DynamoDb db, IDateTime dateTime) : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
    {
        var maybePage = await GetPage(Query.Token, ct);
        if (!maybePage.TryUnwrap(out var page))
        {
            return Either.Left(DomainError.NotFound(Query.Token));
        }

        var request = new QueryRequest();
        request.TableName = db.Table;
        request.IndexName = "gsi1";
        request.Limit = page.PageSize;
        request.KeyConditionExpression = "#pk = :pk AND begins_with(#sk, :prefix)";
        request.ExpressionAttributeNames = new Dictionary<string, string>
        {
            ["#pk"] = "gsi1pk",
            ["#sk"] = "gsi1sk",
        };
        request.ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            [":pk"] = new AttributeValue { S = "APIKEYS" },
            [":prefix"] = new AttributeValue { S = "APIKEY#" },
        };
        if (page.LastEvaluatedKey.TryUnwrap(out var startKey))
        {
            request.ExclusiveStartKey = startKey.ToDictionary(kv => kv.Key, kv => new AttributeValue { S = kv.Value });
        }

        var response = await db.Client.QueryAsync(request, ct);

        Maybe<string> nextPageToken = Maybe.None;
        if (response.LastEvaluatedKey.Count != 0)
        {
            var pageDto = new PageDto
            {
                PageSize = page.PageSize,
                LastEvaluatedKey = response.LastEvaluatedKey.ToDictionary(kv => kv.Key, kv => kv.Value.S),
            };
            nextPageToken = Maybe.Some(await CreateNextPage(pageDto, ct));
        }

        var apiKeys = response.Items
            .Select(item => new ApiKey
            {
                Id = Guid.Parse(item["id"].S),
                Name = item["name"].S,
                Roles = item["roles"].SS,
                CreatedAt = DateTimeOffset.Parse(item["created_at"].S),
            })
            .ToList();

        var result = new QueryResult
        {
            ApiKeys = apiKeys,
            NextPageToken = nextPageToken,
        };
        return Either.Right(result);
    }

    private async Task<Maybe<Page>> GetPage(string token, CancellationToken ct)
    {
        var request = new GetItemRequest();
        request.TableName = db.Table;
        request.Key = new Dictionary<string, AttributeValue>
        {
            ["pk"] = new AttributeValue { S = $"PAGINATOR#{token}" },
            ["sk"] = new AttributeValue { S = $"PAGINATOR#{token}" },
        };

        var response = await db.Client.GetItemAsync(request, ct);
        if (!response.IsItemSet)
        {
            return Maybe.None;
        }

        var pageDto = JsonSerializer.Deserialize<PageDto>(response.Item["data"].S);
        var page = new Page
        {
            PageSize = pageDto.PageSize,
            LastEvaluatedKey = pageDto.LastEvaluatedKey.ToMaybe(),
        };
        return Maybe.Some(page);
    }

    private async Task<string> CreateNextPage(PageDto dto, CancellationToken ct)
    {
        var token = Guid.NewGuid().ToString();
        var expiration = dateTime.Now.AddMinutes(5);

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

        return token;
    }
}
