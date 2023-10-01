namespace WebApi;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithFeatureFlags(
        this RouteHandlerBuilder builder,
        params FeatureFlag[] flags)
    {
        return builder.WithMetadata(flags);
    }
}
