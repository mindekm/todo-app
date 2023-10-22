namespace WebApi.Features.TodoApp.V3.EF.Notes.Delete;

using Microsoft.EntityFrameworkCore;
using OneOf;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;
using Void = Utilities.Void;

public sealed class Handler(TodoAppContext context) : CommandHandler<Command, OneOf<Void, ResourceNotFound>>
{
    protected override async ValueTask<OneOf<Void, ResourceNotFound>> Handle(CancellationToken ct)
    {
        var entity = await context.Notes
            .Where(n => n.CreatedBy == Command.UserId)
            .FirstOrDefaultAsync(n => n.ExternalId == Command.Id, ct);

        if (entity is null)
        {
            return new ResourceNotFound(Command.Id);
        }

        context.Notes.Remove(entity);
        await context.SaveChangesAsync(ct);
        return Void.Value;
    }
}
