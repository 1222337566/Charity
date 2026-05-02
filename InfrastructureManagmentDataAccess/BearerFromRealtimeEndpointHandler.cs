using InfrastructureManagmentDataAccess;
using System.Net.Http.Headers;

public class BearerFromRealtimeEndpointHandler : DelegatingHandler
{
    private readonly ICurrentUserTokenFetcher _fetcher;
    public BearerFromRealtimeEndpointHandler(ICurrentUserTokenFetcher fetcher) => _fetcher = fetcher;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await _fetcher.GetTokenFromSelfAsync(ct); // ← من /auth/realtime
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}
