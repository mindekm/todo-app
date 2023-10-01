namespace WebApi.FeatureHandlers;

public abstract class QueryHandler<TQuery, TResult> : FeatureHandler
{
    protected TQuery Query { get; private set; }

    public virtual async ValueTask<TResult> Handle(TQuery query, CancellationToken ct)
    {
        Query = query;

        return await Handle(ct);
    }

    protected abstract ValueTask<TResult> Handle(CancellationToken ct);
}
