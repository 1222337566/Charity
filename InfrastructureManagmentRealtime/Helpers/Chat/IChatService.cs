using InfrastructureManagmentCore.Domains.Chat;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;

namespace InfrastructureManagmentCore.Services.Chat;

public interface IChatService
{
    Task SaveRoomMessageAsync(string room, string fromUserId, string text, CancellationToken ct = default);
    Task<List<ChatMessage2>> GetRoomHistoryAsync(string room, int take, CancellationToken ct = default);
}

