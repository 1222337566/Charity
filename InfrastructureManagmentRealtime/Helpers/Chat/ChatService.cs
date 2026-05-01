using InfrastructureManagmentCore.Domains.Chat;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Services.Chat;

public class ChatService : IChatService
{
    private readonly IChatRepository _repo;
    public ChatService(IChatRepository repo) => _repo = repo;

    public async Task SaveRoomMessageAsync(string room, string fromUserId, string text, CancellationToken ct = default)
    {
        var r = await _repo.GetOrCreateRoomByNameAsync(room, ct);
        await _repo.AddMessageAsync(new ChatMessage2 { RoomId = r.Id, FromUserId = fromUserId, Text = text, SentAtUtc = DateTime.UtcNow }, ct);
    }

    public Task<List<ChatMessage2>> GetRoomHistoryAsync(string room, int take, CancellationToken ct = default)
        => _repo.GetRoomHistoryAsync(room, take, ct);
}