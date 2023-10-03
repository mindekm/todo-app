namespace WebApi.SwApi;

using Refit;
using WebApi.Data.SwApi;

public interface ISwApiClient
{
    [Get("/people/{id}")]
    Task<PersonDto> GetPersonById(int id, CancellationToken ct);
}
