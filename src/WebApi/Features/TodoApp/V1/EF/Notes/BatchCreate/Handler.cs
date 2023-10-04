namespace WebApi.Features.TodoApp.V1.EF.Notes.BatchCreate;

using WebApi.Data.Ef;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context, IDateTime dateTime) : CommandHandler<Command, List<CommandResult>>
{
    protected override async ValueTask<List<CommandResult>> Handle(CancellationToken ct)
    {
        var result = new List<CommandResult>(Command.Requests.Count);
        var createdAt = dateTime.Now;

        foreach (var request in Command.Requests)
        {
            var entity = new NoteEntity
            {
                ExternalId = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content.UnwrapOrDefault(),
                CreatedBy = Command.UserId,
                CreatedAt = createdAt,
                ModifiedAt = createdAt,
                IsComplete = false,
            };

            context.Notes.Add(entity);
            result.Add(new CommandResult
            {
                Id = entity.ExternalId,
                Title = entity.Title,
            });
        }

        await context.SaveChangesAsync(ct);
        return result;
    }
}
