namespace WebApi.DomainErrors;

public abstract class DomainError
{
    public static DomainError Unauthorized(string message)
        => new Unauthorized(message);

    public static DomainError NotFound(Guid id)
        => new ResourceNotFound(id);
}
