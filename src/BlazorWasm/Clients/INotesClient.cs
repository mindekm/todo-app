using Refit;

namespace BlazorWasm.Clients;

public interface INotesClient
{
    [Post("/api/todo-app/v1/ef/notes/paginator")]
    Task<CreatePaginatorResponse> CreatePaginator(CreatePaginatorRequest request);

    [Get("/api/todo-app/v1/ef/notes/paginator")]
    Task<GetPageResponse> GetPage(string token);

    [Post("/api/todo-app/v1/ef/notes")]
    Task<CreateNoteResponse> Create(CreateNoteRequest request);

    [Post("/api/todo-app/v1/ef/notes/{id}:complete")]
    Task Complete(Guid id);

    [Delete("/api/todo-app/v1/ef/notes/{id}")]
    Task Delete(Guid id);
}