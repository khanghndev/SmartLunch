namespace Khoa_Luan_KS_Web.Areas.Admin.Models;

public sealed class RolePermissionIndexViewModel
{
    public List<RoleListItemVm> Roles { get; set; } = new();
    public List<PermissionModuleVm> Modules { get; set; } = new();
    public List<string> Actions { get; set; } = new();
    public Dictionary<string, Dictionary<string, int>> PermissionMap { get; set; } = new();
}

public sealed class RoleListItemVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; }
}

public sealed class PermissionModuleVm
{
    public string Name { get; set; } = string.Empty;
    public List<string> Resources { get; set; } = new();
}
