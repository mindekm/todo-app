namespace WebApi.Features.TodoApp.V1.Dapper.Delete;

using global::Dapper;
using Npgsql;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IConfiguration configuration) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("Pg"));

        const string sql =
            """
            DELETE FROM "Notes"
            WHERE "ExternalId" = @ExternalId AND "CreatedBy" = @CreatedBy
            """;

        var parameters = new
        {
            ExternalId = Command.Id,
            CreatedBy = Command.UserId,
        };

        var rows = await connection.ExecuteAsync(sql, parameters);
        return rows == 1
            ? Maybe.None
            : Maybe.Some(DomainError.NotFound(Command.Id));
    }
}
