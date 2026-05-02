using Microsoft.AspNetCore.SignalR;

public class NameIdUserIdProvider : IUserIdProvider
{
    public virtual string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
}