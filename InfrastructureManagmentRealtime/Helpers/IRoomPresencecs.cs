namespace InfrastructureManagmentRealtime.Helpers
{
    public interface IRoomPresencecs
    {
        Task AddAsync(string room, string userId, string userName);
        Task RemoveAsync(string room, string userId);
        Task<IReadOnlyList<(string userId, string userName)>> ListAsync(string room);
    }
}
