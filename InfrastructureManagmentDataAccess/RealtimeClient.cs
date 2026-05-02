using Skote.Helpers;
using System.Net.Http.Json;

public class RealtimeClient : IRealtimeClient
{
    private readonly HttpClient _http;
    public RealtimeClient(HttpClient http) => _http = http;

    public Task ToUserAsync(string userId, string type, string message, object payload, CancellationToken ct = default)
        => _http.PostAsJsonAsync($"api/realtime/user/{userId}", new { type, message, payload }, ct);

    public Task BroadcastAsync(string type, string message, object payload, CancellationToken ct = default)
        => _http.PostAsJsonAsync("api/realtime/broadcast", new { type, message, payload }, ct);
}