﻿namespace WebApi.Features.TodoApp.V1.Dapper.GetById;

public sealed class QueryResultDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ModifiedAt { get; set; }

    public bool IsComplete { get; set; }
}
