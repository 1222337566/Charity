namespace InfrastructureManagmentCore.Domains.Chat;

public class ChatMessage2
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid RoomId { get; set; }
    public ChatRoom Room { get; set; } = default!;

    public string FromUserId { get; set; } = default!; // FK -> AspNetUsers.Id
    public string? ToUserId { get; set; }              // للرسائل الخاصة (اختياري)

    public string Text { get; set; } = default!;
    public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
}
