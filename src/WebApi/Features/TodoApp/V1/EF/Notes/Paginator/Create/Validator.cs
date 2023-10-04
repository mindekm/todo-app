namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.Create;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        RuleFor(r => r.MaxPageSize)
            .InclusiveBetween(1, 100);
    }
}
