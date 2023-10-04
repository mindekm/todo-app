namespace WebApi;

public sealed record FeatureFlag(string Name)
{
    public static readonly FeatureFlag DynamoDb = new FeatureFlag("DynamoDb");

    public static readonly FeatureFlag TodoAppV1 = new FeatureFlag("TodoAppV1");

    public static readonly FeatureFlag Redis = new FeatureFlag("Redis");

    public static readonly FeatureFlag Ef = new FeatureFlag("Ef");
}
