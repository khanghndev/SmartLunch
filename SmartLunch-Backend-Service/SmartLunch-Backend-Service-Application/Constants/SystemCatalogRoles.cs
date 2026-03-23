namespace SmartLunch.Backend.Service.Application.Constants;

/// <summary>
/// Gợi ý tên role trong DB để lọc danh mục nhân viên / khách (so khớp không phân biệt hoa thường).
/// </summary>
public static class SystemCatalogRoles
{
    public const string Staff = "Staff";
    /// <summary>Tên role tiếng Việt có thể dùng trong DB.</summary>
    public const string NhanVien = "Nhân viên";
    /// <summary>Nhân viên bán hàng / POS.</summary>
    public const string Sales = "Sales";
    public const string NhanVienBan = "Nhân viên bán";
    public const string Customer = "Customer";
    /// <summary>Đại diện công ty / đơn vị B2B.</summary>
    public const string Company = "Company";
    public const string CongTy = "Công ty";
    /// <summary>Khách hàng (có thể trùng tên role trong DB).</summary>
    public const string KhachHang = "Khách hàng";
}