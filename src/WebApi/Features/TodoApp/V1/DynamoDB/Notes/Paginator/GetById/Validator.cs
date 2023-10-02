namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.Paginator.GetById;

using FluentValidation;

public sealed class Validator : AbstractValidator<Parameters>
{
    public Validator()
    {
        RuleFor(r => r.Token)
            .NotEmpty();
    }
}
