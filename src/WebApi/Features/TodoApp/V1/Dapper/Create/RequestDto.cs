namespace WebApi.Features.TodoApp.V1.Dapper.Create;

using WebApi.IdempotentRequests;

public sealed class RequestDto : IIdempotentRequest
{
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [StringLength(1000)]
    public string Content { get; set; }

    public Guid? IdempotencyKey { get; set; }
}
