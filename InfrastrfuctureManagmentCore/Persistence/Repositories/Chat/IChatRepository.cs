using InfrastructureManagmentCore.Domains.Chat;
using InfrastructureManagmentCore.Domains.Reactions;

namespace InfrastructureManagmentCore.Persistence.Repositories.Abstractions;

public interface IChatRepository
{
    Task<ChatRoom> GetOrCreateRoomByNameAsync(string name, CancellationToken ct = default);
    Task AddMessageAsync(ChatMessage2 m, CancellationToken ct = default);
    Task<List<ChatMessage2>> GetRoomHistoryAsync(string room, int take, CancellationToken ct = default);
}