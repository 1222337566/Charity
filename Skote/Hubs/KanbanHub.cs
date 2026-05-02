using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using InfrastructureManagmentCore.Kanban;
using InfrastructureManagmentInfrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace skote.Hubs
{
    [Authorize]
    public class KanbanHub : Hub
    {
        private readonly IBoardAccessRepository _boards;
        public KanbanHub(IBoardAccessRepository boards) => _boards = boards;
        private static readonly ConcurrentDictionary<string, string?> ConnRoom = new();
        public override Task OnConnectedAsync()
        {
            ConnRoom.TryAdd(Context.ConnectionId, null);
            var list = Context.User?.Claims?.Select(c => $"{c.Type}={c.Value}");
            Console.WriteLine(string.Join(", ", list ?? Array.Empty<string>()));
            try
            {
                return base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HUB] OnConnectedAsync error: " + ex);
                throw;
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            ConnRoom.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }
        /// <summary>انضمام إلى جروب اللوح بعد التحقق من العضوية.</summary>
        public async Task JoinBoard(string boardIdStr)
        {
            if (!Guid.TryParse(boardIdStr, out var boardId)) return;
            var userId = Context.GetUserId();
            if (!await _boards.IsMemberAsync(boardId, userId))
                throw new HubException("Forbidden"); // لا تفصح عن تفاصيل أكتر

            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(boardId));
            await Clients.Caller.SendAsync("JoinedBoard", new { boardId = boardId.ToString() });
        }

        /// <summary>مغادرة جروب اللوح (اختياري).</summary>
        public async Task LeaveBoard(Guid boardId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(boardId));
            await Clients.Caller.SendAsync("LeftBoard", new { boardId });
        }

        /// <summary>انضمام لأكثر من لوح مرّة واحدة (اختياري).</summary>
        public async Task JoinBoards(Guid[] boardIds)
        {
            var userId = Context.GetUserId();
            foreach (var id in boardIds ?? Array.Empty<Guid>())
            {
                if (await _boards.IsMemberAsync(id, userId))
                    await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(id));
            }
            await Clients.Caller.SendAsync("JoinedBoards", new { count = boardIds?.Length ?? 0 });
        }

        private static string GroupName(Guid boardId) => $"board:{boardId}";

        /// <summary>بث تحرك مهمة إلى أعضاء اللوح — يُستدعى من Client مباشرة</summary>
        public async Task BroadcastTaskMove(string boardIdStr, string taskId, string newStatus)
        {
            if (!Guid.TryParse(boardIdStr, out var boardId)) return;
            var userId = Context.GetUserId();
            if (!await _boards.IsMemberAsync(boardId, userId)) return;
            await Clients.OthersInGroup(GroupName(boardId))
                .SendAsync("TaskMoved", new { taskId, newStatus, movedBy = userId });
        }
    }
}
