using System.ComponentModel.DataAnnotations;

namespace Skote.ViewModels.Security;

public class EditUserPermissionsVm
{
    [Required]
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public List<RoleSelectionVm> Roles { get; set; } = new();
    public List<PermissionSelectionVm> Permissions { get; set; } = new();
}
