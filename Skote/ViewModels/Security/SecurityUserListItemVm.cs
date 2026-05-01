namespace Skote.ViewModels.Security;

public class SecurityUserListItemVm
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> DirectPermissions { get; set; } = new();
}
