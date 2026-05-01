using System;
using System.Threading.Tasks;
using InfrastructureManagmentCore.Kanban;
using InfrastructureManagmentCore.Persistence.Repositories;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentInfrastructure.Kanban
{
    public class BoardAccessRepository : IBoardAccessRepository
    {
        private readonly AppDbContext _db;
        public BoardAccessRepository(AppDbContext db) => _db = db;

        public async Task<bool> IsMemberAsync(Guid boardId, string userId)
        {
            return await _db.boardUsers
                .AsNoTracking()
                .AnyAsync(x => x.BoardId == boardId && x.UserId == userId);
        }

        public async Task EnsureMemberAsync(Guid boardId, string userId)
        {
            if (!await IsMemberAsync(boardId, userId))
                throw new UnauthorizedAccessException("You are not a member of this board.");
        }
    }
}