namespace Skote.Helpers
{
    public interface IRealtimeClient
    {
        Task BroadcastAsync(string type, string message, object payload, CancellationToken ct = default);
        Task ToUserAsync(string userId, string type, string message, object payload, CancellationToken ct = default);
    }
}
