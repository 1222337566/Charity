namespace InfrastructureManagmentCore.Domains.Notify;

public class Notification2
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public string? Kind { get; set; }          // مثلاً: calendar-invite / calendar-reminder / ...
    public string? MetaJson { get; set; }
    public bool IsRead { get; set; } = false;
    public string? Level { get; set; } = "info"; // info|success|warning|error
    public string Scope { get; set; } = "user";  // user|topic|broadcast
    public string? CreatedByUserId { get; set; } // FK -> AspNetUsers.Id
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<NotificationDelivery> Deliveries { get; set; } = new List<NotificationDelivery>();
}