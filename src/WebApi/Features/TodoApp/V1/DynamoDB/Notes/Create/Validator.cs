﻿namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Create;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        RuleFor(r => r.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100);

        Unless(r => r.Content is null, () =>
        {
            RuleFor(r => r.Content)
                .MaximumLength(1000);
        });
    }
}