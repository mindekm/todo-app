namespace WebApi;

public sealed record FeatureFlag(string Name)
{
    public static readonly FeatureFlag DynamoDb = new FeatureFlag("DynamoDb");
}
