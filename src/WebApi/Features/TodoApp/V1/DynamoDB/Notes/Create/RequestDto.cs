﻿namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

using WebApi.IdempotentRequests;

public sealed class RequestDto : IIdempotentRequest
{
    public string Title { get; set; }

    public string Content { get; set; }

    public Guid? IdempotencyKey { get; set; }
}
