namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator()
    {
        RuleFor(r => r.MaxPageSize)
            .InclusiveBetween(1, 100);
    }
}
