namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using FluentValidation;

public sealed class Validator : AbstractValidator<Parameters>
{
    public Validator()
    {
        RuleFor(r => r.Token)
            .NotEmpty();
    }
}
