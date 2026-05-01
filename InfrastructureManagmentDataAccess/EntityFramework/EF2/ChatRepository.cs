using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore.Domains.Chat;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Domains.Reactions;
using InfrastructureManagmentDataAccess.EntityFramework;

namespace InfrastructureManagmentCore.Persistence.Repositories.Ef;

public class ChatRepository : IChatRepository
{
    private readonly AppDbContext _db;
    public ChatRepository(AppDbContext db) => _db = db;

    public async Task<ChatRoom> GetOrCreateRoomByNameAsync(string name, CancellationToken ct = default)
    {
        var r = await _db.ChatRooms.SingleOrDefaultAsync(x => x.Name == name, ct);
        if (r is null)
        {
            r = new ChatRoom { Name = name };
            _db.ChatRooms.Add(r);
            await _db.SaveChangesAsync(ct);
        }
        return r;
    }

    public async Task AddMessageAsync(ChatMessage2 m, CancellationToken ct = default)
    {
        _db.ChatMessages.Add(m);
        await _db.SaveChangesAsync(ct);
    }

    public Task<List<ChatMessage2>> GetRoomHistoryAsync(string room, int take, CancellationToken ct = default)
    {
        return _db.ChatMessages
            .AsNoTracking()
            .Where(m => m.Room.Name == room)
            .OrderByDescending(m => m.SentAtUtc)
            .Take(Math.Clamp(take, 10, 200))
            .ToListAsync(ct);
    }
}
