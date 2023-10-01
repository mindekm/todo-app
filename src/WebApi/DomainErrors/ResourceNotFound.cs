namespace WebApi.DomainErrors;

public sealed class ResourceNotFound : DomainError
{
    public ResourceNotFound(string id)
    {
        Id = id;
    }

    public ResourceNotFound(Guid id)
    {
        Id = id.ToString();
    }

    public string Id { get; }
}
