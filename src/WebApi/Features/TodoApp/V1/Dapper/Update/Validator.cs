namespace WebApi.Features.TodoApp.V1.Dapper.Update;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        Unless(r => r.Content is null, () =>
        {
            RuleFor(r => r.Content)
                .MaximumLength(1000);
        });
    }
}
