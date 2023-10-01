namespace WebApi.DomainErrors;

public sealed class Unauthorized : DomainError
{
    public Unauthorized(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
