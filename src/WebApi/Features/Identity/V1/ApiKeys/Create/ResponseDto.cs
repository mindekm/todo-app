namespace WebApi.Features.Identity.V1.ApiKeys.Create;

public sealed class ResponseDto
{
    public Guid Id { get; set; }

    public string Key { get; set; }
}
