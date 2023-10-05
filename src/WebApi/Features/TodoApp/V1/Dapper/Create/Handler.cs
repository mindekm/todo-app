namespace WebApi.Features.TodoApp.V1.Dapper.Create;

using global::Dapper;
using Npgsql;
using WebApi.FeatureHandlers;

public sealed class Handler(IConfiguration configuration, IDateTime dateTime) : CommandHandler<Command, Guid>
{
    protected override async ValueTask<Guid> Handle(CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString("Pg"));

        const string sql =
            """
            INSERT INTO "Notes" ("ExternalId", "Title", "Content", "CreatedBy", "CreatedAt", "ModifiedAt", "IsComplete")
            VALUES (@ExternalId, @Title, @Content, @CreatedBy, @CreatedAt, @CreatedAt, @IsComplete)
            """;

        var createdAt = dateTime.Now;
        var parameters = new
        {
            ExternalId = Guid.NewGuid(),
            Title = Command.Title,
            Content = Command.Content.UnwrapOrDefault(),
            CreatedBy = Command.UserId,
            CreatedAt = createdAt,
            IsComplete = false,
        };
        await connection.ExecuteAsync(sql, parameters);

        return parameters.ExternalId;
    }
}
