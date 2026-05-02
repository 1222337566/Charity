using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

public interface INotificationsClient
{
    Task Receive(string type, string message, object payload); // حدث موحّد
}

[Authorize] // لازم JWT صالح للاتصال
public class NotificationsHub : Hub<INotificationsClient>
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");

        var roles = Context.User?.FindAll(ClaimTypes.Role)?.Select(r => r.Value) ?? Enumerable.Empty<string>();
        foreach (var role in roles)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"role:{role}");

        await base.OnConnectedAsync();
    }

    // اشتراك يدوي في جروب مخصّص
    public Task JoinGroup(string group) => Groups.AddToGroupAsync(Context.ConnectionId, group);
    public Task LeaveGroup(string group) => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);

    // اختبار سريع
    public Task Ping(string message) => Clients.Caller.Receive("pong", message, null);
}
