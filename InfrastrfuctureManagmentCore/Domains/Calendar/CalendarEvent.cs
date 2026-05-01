namespace InfrastructureManagmentCore.Domains.Calendar;

public class CalendarEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
    public bool AllDay { get; set; } = false;
    public string? Color { get; set; }
    public string CreatedByUserId { get; set; } = default!; // FK -> AspNetUsers.Id
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public ICollection<CalendarAttendee> Attendees { get; set; } = new List<CalendarAttendee>();
    public ICollection<EventReminder> Reminders { get; set; } = new List<EventReminder>();
}
