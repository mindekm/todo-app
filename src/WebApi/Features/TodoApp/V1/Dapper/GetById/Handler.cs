namespace WebApi.Features.TodoApp.V1.Dapper.GetById;

using global::Dapper;
using Npgsql;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IConfiguration configuration) : QueryHandler<Query, Either<DomainError, QueryResult>>
{
    protected override async ValueTask<Either<DomainError, QueryResult>> Handle(CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("Pg"));

        const string sql =
            """
            SELECT "ExternalId", "Title", "Content", "CreatedAt", "ModifiedAt", "IsComplete"
            FROM "Notes"
            WHERE "ExternalId" = @ExternalId AND "CreatedBy" = @CreatedBy
            LIMIT 1
            """;

        var parameters = new
        {
            ExternalId = Query.Id,
            CreatedBy = Query.UserId,
        };

        var dto = await connection.QueryFirstOrDefaultAsync<QueryResultDto>(sql, parameters);
        if (dto is null)
        {
            return Either.Left(DomainError.NotFound(Query.Id));
        }

        var result = new QueryResult
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content.ToMaybe(NoneWhen.NullOrWhitespace),
            CreatedAt = dto.CreatedAt,
            ModifiedAt = dto.ModifiedAt,
            IsComplete = dto.IsComplete,
        };
        return Either.Right(result);
    }
}
