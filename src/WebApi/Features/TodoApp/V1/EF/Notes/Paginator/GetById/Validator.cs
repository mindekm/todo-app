namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.GetById;

using FluentValidation;

public sealed class Validator : AbstractValidator<Parameters>
{
    public Validator()
    {
        RuleFor(r => r.Token)
            .NotEmpty();
    }
}
