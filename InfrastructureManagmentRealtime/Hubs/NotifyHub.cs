using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace InfrastructureManagementRealtime.Hubs
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class NotifyHub : Hub
    {
        private string UserId => Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Context.ConnectionId;
        public override async Task OnConnectedAsync()
        {
            // اختياري: تلقائيًا ضم المستخدم لغرفته الشخصية
            var uid = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(uid))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{uid}");

            await base.OnConnectedAsync();
        }

        /// ينضم المستخدم لجروب شخصي باسم userId ليستقبل إشعارات مباشرة
        //public async Task JoinPersonal()
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, UserId);
        //}

        public async Task JoinPersonal()
        {
            var uid = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(uid))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{uid}");
        }

        public async Task JoinTopic(string topic)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"topic:{topic}");

        public async Task LeaveTopic(string topic)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"topic:{topic}");
        /// الانضمام لموضوع (topic) عام (مثال: it / ops / alerts)
        //public async Task JoinTopic(string topic)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, $"topic:{topic}");
        //}

        //public async Task LeaveTopic(string topic)
        //{
        //    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"topic:{topic}");
        //}
    }

    // InfrastructureManagementRealtime/Services/INotificationBus.cs
    public record NotificationPayload(
        string? title,
        string? message,
        string? url,
        string? icon,
        string? level,
        long ticks,
        string? kind = null,     // ← جديد (اختياري)
        object? meta = null ,
        Guid? deliveryId =null

    // ← جديد (اختياري) مثلاً: new { eventId = ev.Id }
    );
   
}

