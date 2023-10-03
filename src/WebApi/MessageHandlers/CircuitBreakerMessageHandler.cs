namespace WebApi.MessageHandlers;

using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;

public sealed class CircuitBreakerMessageHandler(IConcurrentPolicyRegistry<string> registry) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Create circuit breaker per host
        return await registry
            .GetOrAdd($"breaker-{request.RequestUri?.Host}", PolicyFactory)
            .ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
    }

    private static AsyncCircuitBreakerPolicy<HttpResponseMessage> PolicyFactory(string key)
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .Or<TimeoutRejectedException>()
            .CircuitBreakerAsync(10, TimeSpan.FromSeconds(30));
    }
}
