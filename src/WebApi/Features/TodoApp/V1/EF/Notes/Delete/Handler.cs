namespace WebApi.Features.TodoApp.V1.EF.Notes.Delete;

using Microsoft.EntityFrameworkCore;
using Utilities;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context) : CommandHandler<Command, Maybe<DomainError>>
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

        context.Notes.Remove(entity);
        await context.SaveChangesAsync(ct);
        return Maybe.None;
    }
}
