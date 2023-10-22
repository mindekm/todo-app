namespace WebApi.Features.TodoApp.V2.EF.Notes.Update;

using Microsoft.EntityFrameworkCore;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context, IDateTime dateTime) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var entity = await context.Notes
            .Where(n => n.CreatedBy == Command.UserId)
            .FirstOrDefaultAsync(e => e.ExternalId == Command.Id, ct);

        if (entity is null)
        {
            return Maybe.Some(DomainError.NotFound(Command.Id));
        }

        entity.Content = Command.Content.UnwrapOrDefault();
        entity.ModifiedAt = dateTime.Now;
        await context.SaveChangesAsync(ct);
        return Maybe.None;
    }
}
