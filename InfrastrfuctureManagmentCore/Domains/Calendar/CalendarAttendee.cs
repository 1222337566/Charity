namespace InfrastructureManagmentCore.Domains.Calendar;

public class CalendarAttendee
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid EventId { get; set; }
    public CalendarEvent Event { get; set; } = default!;

    public string UserId { get; set; } = default!; // FK -> AspNetUsers.Id
    public string Status { get; set; } = "invited"; // invited|accepted|declined|tentative
}
