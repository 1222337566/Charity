using InfrastructureManagementRealtime.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagementRealtime.Helpers
{
    public interface INotificationBus
    {
        Task ToUserAsync(string userId, NotificationPayload payload);
        Task ToUsersAsync(IEnumerable<string> userIds, NotificationPayload payload);
        Task ToTopicAsync(string topic, NotificationPayload payload);
        Task BroadcastAsync(NotificationPayload payload);
    }
}
