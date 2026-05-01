// Hubs/ChatHub.cs
using InfrastructureManagmentCore.Services.Chat;
using InfrastructureManagmentRealtime.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace InfrastructureManagementRealtime.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    // مهم: JWT فقط
    public class ChatHub : Hub
    {
        private readonly IRoomPresencecs _presence;
        private readonly IChatService _chat;
      
        public ChatHub(IRoomPresencecs presence, IChatService chat) { _presence = presence; _chat = chat; }
        // Presence (in-memory بسيط للتجربة). مع سكِيل أوت هنستبدله بـ Redis Sets.
        // connectionId -> room
        private static readonly ConcurrentDictionary<string, string?> ConnRoom = new();

        private (string userId, string userName) Me =>
            (
                Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.ConnectionId,
                Context.User?.FindFirstValue(ClaimTypes.Name) ?? "User"
            );

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
        public async Task SendToRoom(string room, string message)
        {
            var userId = Context.UserIdentifier ?? Context.ConnectionId;
            var userName = Context.User?.Identity?.Name ?? userId;

            await Clients.Group(room).SendAsync("ReceiveMessage", new
            {
                room,
                user = userName,
                userId,
                message,
                at = DateTime.UtcNow
            });

            await _chat.SaveRoomMessageAsync(room, userId, message);
        }

        public Task JoinRoom(string room) => Groups.AddToGroupAsync(Context.ConnectionId, room);
        public Task LeaveRoom(string room) => Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        // ===== Rooms =====
        //public async Task JoinRoom(string room)
        //{
        //    var (uid, name) = Me;

        //    // خروج من القديم إن وجد
        //    if (ConnRoom.TryGetValue(Context.ConnectionId, out var prev) && !string.IsNullOrEmpty(prev))
        //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, prev!);

        //    await Groups.AddToGroupAsync(Context.ConnectionId, room);
        //    ConnRoom[Context.ConnectionId] = room;

        //    await _presence.AddAsync(room, uid, name);

        //    // ابعت القائمة الحالية للمتصل
        //    var list = await _presence.ListAsync(room);
        //    await Clients.Caller.SendAsync("OnlineUsers", list.Select(x => new { userId = x.userId, userName = x.userName }));

        //    // أعلن لباقي الغرفة
        //    await Clients.Group(room).SendAsync("UserOnline", new { userId = uid, userName = name });
        //}

        //public async Task LeaveRoom(string room)
        //{
        //    var (uid, name) = Me;
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        //    ConnRoom[Context.ConnectionId] = null;

        //    await _presence.RemoveAsync(room, uid);

        //    var list = await _presence.ListAsync(room);
        //    await Clients.Group(room).SendAsync("OnlineUsers", list.Select(x => new { userId = x.userId, userName = x.userName }));
        //    await Clients.Group(room).SendAsync("UserOffline", new { userId = uid, userName = name });
        //}

        // ===== Messages (من غير user param) =====
        //public async Task SendToRoom(string room, string message)
        //{
        //    var (_, name) = Me;
        //    await Clients.Group(room).SendAsync("ReceiveMessage",
        //        new { room, fromName = name, message, at = DateTime.UtcNow });
        //}

        public async Task Typing(string room, bool isTyping)
        {
            var (_, name) = Me;
            await Clients.OthersInGroup(room).SendAsync("UserTyping",
                new { room, userName = name, isTyping });
        }

        public async Task SendPrivate(string toUserId, string message)
        {
            var (_, name) = Me;
            await Clients.User(toUserId).SendAsync("ReceivePrivate",
                new { fromName = name, message, at = DateTime.UtcNow });
        }

        // (اختياري) لو محتاج ترجع قائمة المتواجدين بمنتهى البساطة (in-memory):
        // لاحظ: في سكِيل أوت ده مش دقيق، استبدله بـ Redis كما بالأسفل.
        public Task<IEnumerable<object>> GetUsersInRoom(string room)
        {
            // تجميعة تقريبية based on connections (للديمو فقط)
            var users = ConnRoom
                .Where(kv => kv.Value == room)
                .Select(kv => kv.Key) // connectionIds
                .Select(cid => new { userId = cid, userName = "User" }) // بلا مخزن أسماء مركزي هنا
                .Cast<object>();
            return Task.FromResult(users);
        }
    }
}
