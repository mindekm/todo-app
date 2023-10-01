namespace WebApi;

public sealed class UnhandledDomainErrorException : Exception
{
    public UnhandledDomainErrorException()
    {
    }

    public UnhandledDomainErrorException(string message)
        : base(message)
    {
    }
}
