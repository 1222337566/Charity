using InfrastructureManagmentCore.Domains.Reactions;

namespace InfrastructureManagmentCore.Domains.Notify;

public class NotificationDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid NotificationId { get; set; }
    public Notification2 Notification { get; set; } = default!;

    public string UserId { get; set; } = default!;   // FK -> AspNetUsers.Id
    public DateTime? CreatedAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsRead { get; set; }
}
