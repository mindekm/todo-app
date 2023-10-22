namespace WebApi.Features.TodoApp.V3.EF.Notes.Update;

using Microsoft.EntityFrameworkCore;
using OneOf;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;
using Void = Utilities.Void;

public sealed class Handler(TodoAppContext context, IDateTime dateTime)
    : CommandHandler<Command, OneOf<Void, ResourceNotFound, InvalidOperation>>
{
    protected override async ValueTask<OneOf<Void, ResourceNotFound, InvalidOperation>> Handle(CancellationToken ct)
    {
        var entity = await context.Notes
            .Where(n => n.CreatedBy == Command.UserId)
            .FirstOrDefaultAsync(e => e.ExternalId == Command.Id, ct);

        if (entity is null)
        {
            return new ResourceNotFound(Command.Id);
        }

        if (entity.IsComplete)
        {
            return new InvalidOperation();
        }

        entity.Content = Command.Content.UnwrapOrDefault();
        entity.ModifiedAt = dateTime.Now;
        await context.SaveChangesAsync(ct);
        return Void.Value;
    }
}
