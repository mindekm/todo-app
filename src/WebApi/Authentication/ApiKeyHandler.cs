namespace WebApi.Authentication;

using System.Security.Claims;
using System.Text.Encodings.Web;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public sealed class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
{
    private readonly DynamoDb db;

    public ApiKeyHandler(
        IOptionsMonitor<ApiKeyOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        DynamoDb db)
        : base(options, logger, encoder)
    {
        this.db = db;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headerValue = Request.Headers.Authorization.ToString();
        if (!headerValue.StartsWith("ApiKey ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.NoResult();
        }

        var key = headerValue.Substring("ApiKey ".Length).Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            return AuthenticateResult.NoResult();
        }

        var request = new GetItemRequest
        {
            TableName = db.Table,
            Key = new Dictionary<string, AttributeValue>
            {
                ["pk"] = new AttributeValue { S = $"APIKEY#{key}" },
                ["sk"] = new AttributeValue { S = $"APIKEY#{key}" },
            },
        };

        var response = await db.Client.GetItemAsync(request);
        if (!response.IsItemSet)
        {
            return AuthenticateResult.Fail("Invalid key");
        }

        var identity = new ClaimsIdentity(ApiKeyDefaults.AuthenticationScheme);
        foreach (var role in response.Item["roles"].SS)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, response.Item["id"].S));
        identity.AddClaim(new Claim(ClaimTypes.Name, response.Item["name"].S));

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ApiKeyDefaults.AuthenticationScheme);

        return AuthenticateResult.Success(ticket);
    }
}
