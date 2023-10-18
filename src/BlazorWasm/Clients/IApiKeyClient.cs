using Refit;

namespace BlazorWasm.Clients;

public interface IApiKeyClient
{
    [Post("/api/identity/v1/api-keys/paginator")]
    Task<CreateApiKeyPaginatorResponse> CreatePaginator(CreateApiKeyPaginatorRequest request);

    [Get("/api/identity/v1/api-keys/paginator")]
    Task<GetApiKeyPageResponse> GetPage(string token);

    [Delete("/api/identity/v1/api-keys/{id}")]
    Task Delete(Guid id);

    [Post("/api/identity/v1/api-keys")]
    Task<CreateApiKeyResponse> Create(CreateApiKeyRequest request);
}