﻿namespace WebApi.Features.TodoApp.V1.EF.Notes.BatchCreate;

public sealed class CreateNoteDto
{
    public string Title { get; set; }

    public string Content { get; set; }
}