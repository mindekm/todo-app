namespace WebApi.Features.TodoApp.V1.EF.Notes.Complete;

using Microsoft.EntityFrameworkCore;
using Utilities;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context, IDateTime dateTime) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var entity = await context.Notes
            .Where(n => n.CreatedBy == Command.UserId)
            .FirstOrDefaultAsync(n => n.ExternalId == Command.Id, ct);

        if (entity is null)
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        if (entity.IsComplete)
        {
            return Maybe.None;
        }

        entity.IsComplete = true;
        entity.ModifiedAt = dateTime.Now;
        await context.SaveChangesAsync(ct);
        return Maybe.None;
    }
}
