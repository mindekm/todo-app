﻿namespace WebApi.Features.TodoApp.V1.Dapper.Complete;

public sealed class Command
{
    public required Guid Id { get; init; }

    public required string UserId { get; init; }
}
