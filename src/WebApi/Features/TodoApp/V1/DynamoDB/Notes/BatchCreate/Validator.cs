namespace WebApi.Features.TodoApp.V1.DynamoDB.Notes.BatchCreate;

using FluentValidation;

public sealed class Validator : AbstractValidator<RequestDto>
{
    public Validator(IValidator<CreateNoteDto> inner)
    {
        RuleFor(r => r.Requests.Count)
            .LessThanOrEqualTo(50);

        RuleForEach(r => r.Requests)
            .SetValidator(inner);
    }

    public sealed class CreateNoteValidator : AbstractValidator<CreateNoteDto>
    {
        public CreateNoteValidator()
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
}
