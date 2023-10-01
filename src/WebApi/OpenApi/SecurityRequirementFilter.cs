namespace WebApi;

using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Authentication;

public sealed class SecurityRequirementFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var policies = new List<string>();

        foreach (var attribute in context.ApiDescription.ActionDescriptor.EndpointMetadata.OfType<AuthorizeAttribute>())
        {
            policies.Add(attribute.Policy);
        }

        if (policies.Count == 0)
        {
            return;
        }

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = ApiKeyDefaults.AuthenticationScheme,
            },
        };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                [scheme] = policies,
            },
        };
    }
}
