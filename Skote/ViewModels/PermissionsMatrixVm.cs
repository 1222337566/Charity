namespace Skote.ViewModels.Security;

public sealed class PermissionsMatrixVm
{
    public string CurrentUserName { get; set; } = string.Empty;
    public List<string> CurrentUserRoles { get; set; } = new();
    public List<string> CurrentUserPermissions { get; set; } = new();
    public List<ModulePolicyVm> Policies { get; set; } = new();
    public List<ControllerAccessVm> ControllerAccesses { get; set; } = new();
}

public sealed class ModulePolicyVm
{
    public string Group { get; set; } = string.Empty;
    public string PolicyName { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;
    public string DefaultRolesCsv { get; set; } = string.Empty;
}

public sealed class ControllerAccessVm
{
    public string Group { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ViewPolicy { get; set; } = string.Empty;
    public string ManagePolicy { get; set; } = string.Empty;
}
