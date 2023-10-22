namespace WebApi.Features.TodoApp.V3.EF.Notes.GetById;

using Microsoft.EntityFrameworkCore;
using OneOf;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context)
    : QueryHandler<Query, OneOf<QueryResult, ResourceNotFound, InvalidOperation>>
{
    protected override async ValueTask<OneOf<QueryResult, ResourceNotFound, InvalidOperation>> Handle(CancellationToken ct)
    {
        var entity = await context.Notes
            .AsNoTracking()
            .Where(n => n.CreatedBy == Query.UserId)
            .Select(n => new
            {
                n.ExternalId,
                n.Title,
                n.Content,
                n.CreatedAt,
                n.ModifiedAt,
                n.IsComplete,
            })
            .FirstOrDefaultAsync(n => n.ExternalId == Query.Id, ct);

        if (entity is null)
        {
            return new ResourceNotFound(Query.Id);
        }

        // Unreachable, example only
        if (false)
        {
            return new InvalidOperation();
        }

        return new QueryResult
        {
            Id = entity.ExternalId,
            Title = entity.Title,
            Content = entity.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            CreatedAt = entity.CreatedAt,
            ModifiedAt = entity.ModifiedAt,
            IsComplete = entity.IsComplete,
        };
    }
}
