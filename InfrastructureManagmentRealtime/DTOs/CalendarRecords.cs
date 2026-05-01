namespace InfrastructureManagmentRealtime.DTOs
{
   // public record RespondDto(Guid EventId, string Status);
   // public record CreateEventDto(string Title, DateTime StartUtc, DateTime EndUtc, List<string> AttendeesUserIds);
  

 

  

 public record CreateEventDto(
    string Title,
    DateTime StartUtc,
    DateTime? EndUtc,
    List<string> AttendeesUserIds);

    public record UpdateEventDto(
        Guid Id,
        string Title,
        DateTime StartUtc,
        DateTime? EndUtc);

    public record RespondDto(
        Guid EventId,
        string Status); // accepted | declined | tentative

    public record CalendarEventDto(
        Guid Id,
        string Title,
        DateTime StartUtc,
        DateTime? EndUtc,
        bool AllDay = false,
        string? Color = null);

    public sealed class CalendarRespondDto
    {
        public Guid EventId { get; set; }
        public string Status { get; set; } = "accepted"; // accepted | declined | tentative
    }
}
