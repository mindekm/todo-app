﻿namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

using WebApi.IdempotentRequests;

public sealed class RequestDto : IIdempotentRequest
{
    public List<CreateNoteDto> Requests { get; set; } = new List<CreateNoteDto>();

    public Guid? IdempotencyKey { get; set; }
}
