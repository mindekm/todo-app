namespace WebApi.Features.Identity.V1.ApiKeys.Create;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        RuleFor(r => r.Name)
            .NotEmpty();
    }
}
