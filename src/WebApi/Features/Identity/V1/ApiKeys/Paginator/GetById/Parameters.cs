namespace WebApi.Features.Identity.V1.ApiKeys.Paginator.GetById;

using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

public struct Parameters
{
    [FromQuery]
    [SwaggerParameter(Required = true)]
    public string Token { get; set; }
}
