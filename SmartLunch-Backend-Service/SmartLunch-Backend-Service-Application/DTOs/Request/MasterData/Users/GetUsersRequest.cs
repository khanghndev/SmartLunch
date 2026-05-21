namespace SmartLunch.Backend.Service.Application.DTOs.Request.MasterData.Users;

public class GetUsersRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }

    /// <summary>Lọc user có role trùng tên (không phân biệt hoa thường), vd. Staff, Customer.</summary>
    public string? RoleName { get; set; }

    /// <summary>Chỉ user có ít nhất một role vận hành (loại Customer/Organization/Khách hàng).</summary>
    public bool? StaffOnly { get; set; }
}
