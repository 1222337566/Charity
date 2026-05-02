namespace InfrastructureManagmentCore.Domains.Calendar;

public class EventReminder
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EventId { get; set; }
    public CalendarEvent Event { get; set; } = default!;

    public int MinutesBefore { get; set; } = 10;
    public string Channel { get; set; } = "signalr"; // signalr|email...
}