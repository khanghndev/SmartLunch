namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Organizations;

/// <summary>
/// Chỉ cập nhật trạng thái hoạt động của đơn vị khách hàng (B2B). Không chỉnh thông tin liên hệ/MST.
/// </summary>
public class UpdateOrganizationStatusRequest
{
    public bool IsActive { get; set; }
}
