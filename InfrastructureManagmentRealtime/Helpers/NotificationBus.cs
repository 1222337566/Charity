using InfrastructureManagementRealtime.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagementRealtime.Helpers

{
    public class NotificationBus : INotificationBus
    {
        private readonly IHubContext<NotifyHub> _hub;
        public NotificationBus(IHubContext<NotifyHub> hub) => _hub = hub;

        public Task ToUserAsync(string userId, NotificationPayload payload)
            => _hub.Clients.Group($"user:{userId}").SendAsync("ReceiveNotification", payload);

        public Task ToUsersAsync(IEnumerable<string> userIds, NotificationPayload payload)
            => _hub.Clients.Groups(userIds).SendAsync("ReceiveNotification", payload);

        public Task ToTopicAsync(string topic, NotificationPayload payload)
            => _hub.Clients.Group($"topic:{topic}").SendAsync("ReceiveNotification", payload);

        public Task BroadcastAsync(NotificationPayload payload)
            => _hub.Clients.All.SendAsync("ReceiveNotification", payload);
    }
}
