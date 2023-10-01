namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using Microsoft.AspNetCore.Mvc;

public struct Parameters
{
    [FromQuery]
    public string Token { get; set; }
}
