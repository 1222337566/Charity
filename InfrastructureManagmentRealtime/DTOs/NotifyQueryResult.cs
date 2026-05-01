using InfrastructureManagementRealtime.DTOs;

namespace InfrastructureManagmentRealtime.DTOs
{
    public class NotifyQueryResult
    {
        public List<NotificationDeliveryDto> Items { get; set; } = new();
        public bool HasMore { get; set; }
    }
}
