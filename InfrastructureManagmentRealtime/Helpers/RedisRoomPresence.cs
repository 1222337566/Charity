using StackExchange.Redis;

namespace InfrastructureManagmentRealtime.Helpers
{
    public class RedisRoomPresence : IRoomPresencecs
    {
        private readonly IDatabase _db;
        public RedisRoomPresence(IConnectionMultiplexer mux) => _db = mux.GetDatabase();

        private static string Key(string room) => $"presence:room:{room}"; // Hash: userId -> userName

        public async Task AddAsync(string room, string userId, string userName)
            => await _db.HashSetAsync(Key(room), userId, userName);

        public async Task RemoveAsync(string room, string userId)
            => await _db.HashDeleteAsync(Key(room), userId);

        public async Task<IReadOnlyList<(string userId, string userName)>> ListAsync(string room)
        {
            var entries = await _db.HashGetAllAsync(Key(room));
            return entries.Select(e => ((string)e.Name!, (string)e.Value!)).ToList();
        }
    }
}
