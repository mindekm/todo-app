namespace BlazorWasm;

using System.Net.Http.Headers;

public sealed class CredentialsMessageHandler(StateProvider stateProvider) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine(stateProvider.GetHashCode());

        var key = stateProvider.CurrentUser.Claims.FirstOrDefault(c => c.Type == "Key");
        if (key is null)
        {
            throw new InvalidOperationException("Missing key claim");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", key.Value);

        return base.SendAsync(request, cancellationToken);
    }
}
