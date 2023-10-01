namespace WebApi.FeatureHandlers;

public abstract class CommandHandler<TCommand, TResult> : FeatureHandler
{
    protected TCommand Command { get; private set; }

    public virtual async ValueTask<TResult> Handle(TCommand command, CancellationToken ct)
    {
        Command = command;

        return await Handle(ct);
    }

    protected abstract ValueTask<TResult> Handle(CancellationToken ct);
}
