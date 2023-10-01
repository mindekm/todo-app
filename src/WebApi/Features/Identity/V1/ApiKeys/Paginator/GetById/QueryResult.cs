﻿namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using Utilities;

public sealed class QueryResult
{
    public required List<ApiKey> ApiKeys { get; init; }

    public required Maybe<string> NextPageToken { get; init; }
}
