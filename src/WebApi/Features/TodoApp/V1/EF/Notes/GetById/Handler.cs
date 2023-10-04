namespace WebApi.Features.TodoApp.V1.EF.Notes.GetById;

using Microsoft.EntityFrameworkCore;
using Utilities;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(TodoAppContext context) : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
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
            return Either.Left(DomainError.NotFound(Query.Id));
        }

        var result = new QueryResult
        {
            Id = entity.ExternalId,
            Title = entity.Title,
            Content = entity.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            CreatedAt = entity.CreatedAt,
            ModifiedAt = entity.ModifiedAt,
            IsComplete = entity.IsComplete,
        };
        return Either.Right(result);
    }
}
