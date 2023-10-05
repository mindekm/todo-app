namespace WebApi.Features.TodoApp.V1.Dapper.Create;

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

        When(r => r.IdempotencyKey.HasValue, () =>
        {
            RuleFor(r => r.IdempotencyKey.Value)
                .NotEmpty();
        });
    }
}
