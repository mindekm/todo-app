namespace WebApi.DomainErrors;

public sealed class ResourceNotFound : DomainError
{
    public ResourceNotFound(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
