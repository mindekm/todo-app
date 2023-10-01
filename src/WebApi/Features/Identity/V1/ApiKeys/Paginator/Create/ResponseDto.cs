namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.Create;

public sealed class ResponseDto
{
    public string Token { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}
