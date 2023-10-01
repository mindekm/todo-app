namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

public sealed class ResponseDto
{
    public List<ApiKeyDto> ApiKeys { get; set; }

    public string NextPageToken { get; set; }
}
