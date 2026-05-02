using System.Net.Http;

namespace Skote.Helpers;

public class RemoteNotifyClient : IRemoteNotifyClient
{
    private readonly HttpClient _http;

    public RemoteNotifyClient(HttpClient http)
    {
        _http = http;
    }

    public async Task SendTestAsync(CancellationToken ct = default)
    {
        using var response = await _http.PostAsync("notify/test", content: null, ct);
        response.EnsureSuccessStatusCode();
    }
}
