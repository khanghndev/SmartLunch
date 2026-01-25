using MediatR;
using SmartLunch.Backend.Service.Application.DTOs.Response.MasterData.Permissions;

namespace SmartLunch.Backend.Service.Application.Queries.Permissions.GetPermissions;

/// <summary>
/// Query to get list of permissions
/// </summary>
public class GetPermissionsQuery : IRequest<GetPermissionsResponse>
{
    public bool? IsActive { get; set; }
    public string? Resource { get; set; }
    public string? Action { get; set; }

    public GetPermissionsQuery(bool? isActive = null, string? resource = null, string? action = null)
    {
        IsActive = isActive;
        Resource = resource;
        Action = action;
    }
}
