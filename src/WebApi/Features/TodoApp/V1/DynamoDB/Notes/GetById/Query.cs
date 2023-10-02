﻿namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.GetById;

public sealed class Query
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }
}
