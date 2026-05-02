namespace Skote.ViewModels.Security;

public class SecurityUsersVm
{
    public string? Search { get; set; }
    public List<SecurityUserListItemVm> Users { get; set; } = new();
}
