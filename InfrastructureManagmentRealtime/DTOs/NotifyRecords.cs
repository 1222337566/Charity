namespace InfrastructureManagementRealtime.DTOs
{ 
public record ByIdDto(string UserId, string Title, string Message);

public record TopicDto(string Topic, string Title, string Message);
public record BroadcastDto1(string Title, string Message);
// DTOs
public record NotifyUserDto(string UserName, string Title, string Message);
public record NotifyUserIdDto(string UserId, string Title, string Message);
public record NotifyTopicDto(string Topic, string Title, string Message);
public record NotifyBroadcastDto(string Title, string Message);
    public record NotificationDeliveryDto(
    Guid DeliveryId,
    string Title,
    string Message,
    string Level,
    string? Url,
    DateTime? CreatedAtUtc,
    bool IsRead,
    string? Kind,
    object? Meta
);
}