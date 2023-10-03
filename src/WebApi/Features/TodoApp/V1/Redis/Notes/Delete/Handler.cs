namespace WebApi.Features.TodoApp.V1.Redis.Notes.Delete;

using StackExchange.Redis;
using Utilities;
using WebApi.DomainErrors;
using WebApi.FeatureHandlers;

public sealed class Handler(IConnectionMultiplexer multiplexer) : CommandHandler<Command, Maybe<DomainError>>
{
    protected override async ValueTask<Maybe<DomainError>> Handle(CancellationToken ct)
    {
        var db = multiplexer.GetDatabase();

        var isDeleted = await db.KeyDeleteAsync($"User:{Command.UserId}__Note:{Command.Id}");
        return isDeleted ? Maybe.None : Maybe.Some(DomainError.NotFound(Command.Id));
    }
}
