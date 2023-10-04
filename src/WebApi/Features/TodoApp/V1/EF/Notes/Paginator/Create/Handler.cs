namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.Create;

using WebApi.Data.Ef;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context, IDateTime dateTime) : CommandHandler<Command, CommandResult>
{
    protected override async ValueTask<CommandResult> Handle(CancellationToken ct)
    {
        var entity = new PaginatorEntity
        {
            Id = Guid.NewGuid().ToString(),
            PageSize = Command.PageSize,
            LastResult = default,
            ExpiresAt = dateTime.Now.AddMinutes(5),
            SearchCondition = Command.SearchCondition.UnwrapOrDefault(),
        };

        context.Paginators.Add(entity);
        await context.SaveChangesAsync(ct);
        return new CommandResult
        {
            Token = entity.Id,
            ExpiresAt = entity.ExpiresAt,
        };
    }
}
