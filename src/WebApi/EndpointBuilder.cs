namespace WebApi;

using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.DomainErrors;

public abstract class EndpointBuilder : IEndpointBuilder
{
    public abstract void Setup(IEndpointRouteBuilder builder);

    protected static ValidationProblem ValidationProblem(ValidationResult result)
        => TypedResults.ValidationProblem(result.ToDictionary());

    protected static Ok<TResponse> Ok<TResponse>(TResponse response)
        => TypedResults.Ok(response);

    protected static NotFound NotFound()
        => TypedResults.NotFound();

    protected static NoContent NoContent()
        => TypedResults.NoContent();

    protected static Conflict Conflict()
        => TypedResults.Conflict();

    protected static UnauthorizedHttpResult NotAuthorized()
        => TypedResults.Unauthorized();

    protected static IResult MapError(DomainError error)
    {
        return error switch
        {
            ResourceNotFound => NotFound(),
            Unauthorized => NotAuthorized(),
            _ => throw new UnhandledDomainErrorException(),
        };
    }
}
