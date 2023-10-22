namespace WebApi.Features.TodoApp.V3.EF.Notes.Create;

using WebApi.Data.Ef;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context, IDateTime dateTime) : CommandHandler<Command, Guid>
{
    protected override async ValueTask<Guid> Handle(CancellationToken ct)
    {
        var createdAt = dateTime.Now;
        var entity = new NoteEntity
        {
            ExternalId = Guid.NewGuid(),
            Title = Command.Title,
            Content = Command.Content.UnwrapOrDefault(),
            CreatedAt = createdAt,
            ModifiedAt = createdAt,
            CreatedBy = Command.UserId,
            IsComplete = false,
        };

        context.Notes.Add(entity);
        await context.SaveChangesAsync(ct);

        return entity.ExternalId;
    }
}
