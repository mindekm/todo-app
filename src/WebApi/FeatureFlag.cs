namespace WebApi;

public sealed record FeatureFlag(string Name)
{
    public static readonly FeatureFlag DynamoDb = new FeatureFlag("DynamoDb");

    public static readonly FeatureFlag TodoAppV1 = new FeatureFlag("TodoAppV1");

    public static readonly FeatureFlag Redis = new FeatureFlag("Redis");

    public static readonly FeatureFlag Ef = new FeatureFlag("Ef");

    public static readonly FeatureFlag Dapper = new FeatureFlag("Dapper");

    public static readonly FeatureFlag TodoAppV2 = new FeatureFlag("TodoAppV2");

    public static readonly FeatureFlag TodoAppV3 = new FeatureFlag("TodoAppV3");
}
