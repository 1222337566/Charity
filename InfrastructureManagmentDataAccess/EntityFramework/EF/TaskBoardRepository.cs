using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    // Infrastructure/Repos/Kanban/TaskBoardRepository.cs
    public class TaskBoardRepository : ITaskBoardRepository
    {
        private readonly AppDbContext _db;
        public TaskBoardRepository(AppDbContext db) => _db = db;

        public Task<TaskBoard?> GetAsync(Guid id, CancellationToken ct)
            => _db.TaskBoards.Include(b => b.Tasks).FirstOrDefaultAsync(b => b.Id == id, ct);

        public Task<List<TaskBoard>> ListMineAsync(string userId, CancellationToken ct)
            => _db.TaskBoards.Where(b => b.CreatedByUserId == userId).ToListAsync(ct);

        public async Task<TaskBoard> CreateAsync(TaskBoard board, CancellationToken ct)
        {
            _db.TaskBoards.Add(board);

            // ★ أضف المالك Owner تلقائيًا إلى جدول boardUsers
            _db.boardUsers.Add(new InfrastrfuctureManagmentCore.Domains.Kanaban.BoardUser
            {
                BoardId = board.Id,
                UserId = board.CreatedByUserId,
                Role = "Owner"
            });

            await _db.SaveChangesAsync(ct);
            return board;
        }
        public async Task<bool> IsOwnerAsync(Guid boardId, string userId, CancellationToken ct)
        {
            return await _db.boardUsers
                .AsNoTracking()
                .AnyAsync(x => x.BoardId == boardId && x.UserId == userId && x.Role == "Owner", ct);
        }

        public async Task<bool> IsMemberAsync(Guid boardId, string userId, CancellationToken ct)
        {
            return await _db.boardUsers
                .AsNoTracking()
                .AnyAsync(x => x.BoardId == boardId && x.UserId == userId, ct);
        }

        public async Task AddMemberAsync(Guid boardId, string targetUserId, string role, string byUserId, CancellationToken ct)
        {
            if (!await IsOwnerAsync(boardId, byUserId, ct))
                throw new UnauthorizedAccessException("Only owner can add members.");

            var exists = await _db.boardUsers
                .AnyAsync(x => x.BoardId == boardId && x.UserId == targetUserId, ct);
            if (exists) return;

            _db.boardUsers.Add(new InfrastrfuctureManagmentCore.Domains.Kanaban.BoardUser
            {
                BoardId = boardId,
                UserId = targetUserId,
                Role = string.IsNullOrWhiteSpace(role) ? "Member" : role
            });
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveMemberAsync(Guid boardId, string targetUserId, string byUserId, CancellationToken ct)
        {
            if (!await IsOwnerAsync(boardId, byUserId, ct))
                throw new UnauthorizedAccessException("Only owner can remove members.");

            var row = await _db.boardUsers.FirstOrDefaultAsync(x => x.BoardId == boardId && x.UserId == targetUserId, ct);
            if (row == null) return;

            if (row.Role == "Owner")
            {
                var ownerCount = await _db.boardUsers.CountAsync(x => x.BoardId == boardId && x.Role == "Owner", ct);
                if (ownerCount <= 1) throw new InvalidOperationException("Cannot remove the last owner.");
            }

            _db.boardUsers.Remove(row);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<InfrastrfuctureManagmentCore.Domains.Kanaban.BoardUser>> GetMembersAsync(Guid boardId, string byUserId, CancellationToken ct)
        {
            if (!await IsMemberAsync(boardId, byUserId, ct))
                throw new UnauthorizedAccessException("Not a member.");

            return await _db.boardUsers
                .Where(x => x.BoardId == boardId)
                .OrderByDescending(x => x.Role == "Owner")
                .ThenBy(x => x.UserId)
                .ToListAsync(ct);
        }
    }
}
