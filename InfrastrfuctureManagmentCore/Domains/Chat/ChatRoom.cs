namespace InfrastructureManagmentCore.Domains.Chat;

public class ChatRoom
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public bool IsPrivate { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
