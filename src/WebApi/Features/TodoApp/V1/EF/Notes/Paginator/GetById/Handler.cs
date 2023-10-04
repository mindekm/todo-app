namespace WebApi.Features.TodoApp.V1.EF.Notes.Paginator.GetById;

using Microsoft.EntityFrameworkCore;
using Utilities;
using WebApi.Data.Ef;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IDbContextFactory<TodoAppContext> contextFactory, IDateTime dateTime)
    : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        var paginator = await context.Paginators
            .AsNoTracking()
            .Where(p => p.ExpiresAt > dateTime.Now)
            .FirstOrDefaultAsync(p => p.Id == Query.Token, ct);
        if (paginator is null)
        {
            return Either.Left(DomainError.NotFound(Query.Token));
        }

        var query = context.Notes
            .AsNoTracking()
            .OrderBy(n => n.Id)
            .Where(n => n.CreatedBy == Query.UserId);

        if (paginator.SearchCondition is not null)
        {
            query = query
                .Where(n => n.SearchVector.Matches(EF.Functions.ToTsQuery("english", paginator.SearchCondition)));
        }

        if (paginator.LastResult.HasValue)
        {
            query = query.Where(n => n.Id > paginator.LastResult.Value);
        }

        var notes = await query
            .Select(n => new
            {
                n.Id,
                n.ExternalId,
                n.Title,
                n.Content,
                n.CreatedAt,
                n.ModifiedAt,
                n.IsComplete,
            })
            .Take(paginator.PageSize)
            .ToListAsync(ct);

        var nextPage = notes.Count == paginator.PageSize
            ? Maybe.Some(await CreateNextPage(notes[^1].Id, paginator.PageSize, paginator.SearchCondition, ct))
            : Maybe.None;
        var result = new QueryResult
        {
            NextPageToken = nextPage,
            Notes = notes.Select(n => new Note
                {
                    Id = n.ExternalId,
                    Title = n.Title,
                    Content = n.Content.ToMaybe(NoneWhen.NullOrWhitespace),
                    CreatedAt = n.CreatedAt,
                    ModifiedAt = n.ModifiedAt,
                    IsComplete = n.IsComplete,
                })
                .ToList(),
        };
        return Either.Right(result);
    }

    private async Task<string> CreateNextPage(int lastResult, int pageSize, string searchCondition, CancellationToken ct)
    {
        await using var context = await contextFactory.CreateDbContextAsync(ct);

        var entity = new PaginatorEntity
        {
            Id = Guid.NewGuid().ToString(),
            PageSize = pageSize,
            LastResult = lastResult,
            ExpiresAt = dateTime.Now.AddMinutes(5),
            SearchCondition = searchCondition,
        };

        context.Paginators.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
