﻿namespace WebApi.Features.TodoApp.V1.Redis.Notes.BatchCreate;

using WebApi.IdempotentRequests;

public sealed class RequestDto : IIdempotentRequest
{
    [MaxLength(50)]
    public List<CreateNoteDto> Requests { get; set; } = new List<CreateNoteDto>();

    public Guid? IdempotencyKey { get; set; }
}
