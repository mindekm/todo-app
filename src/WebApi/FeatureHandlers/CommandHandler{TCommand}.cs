namespace WebApi.FeatureHandlers;

public abstract class CommandHandler<TCommand> : FeatureHandler
{
    protected TCommand Command { get; private set; }

    public virtual async ValueTask Handle(TCommand command, CancellationToken ct)
    {
        Command = command;

        await Handle(ct);
    }

    protected abstract ValueTask Handle(CancellationToken ct);
}
