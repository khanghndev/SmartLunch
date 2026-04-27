using SmartLunch.Backend.Service.Application.Constants;

namespace SmartLunch.Backend.Service.Application.Helpers;

public static class IntakeProposalAccessHelper
{
    public static bool IsElevatedReviewer(IEnumerable<string> roleNames)
    {
        foreach (var n in roleNames)
        {
            if (string.Equals(n, "Admin", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    /// <summary>Nhân viên kho / Staff có thể lập phiếu đề xuất.</summary>
    public static bool CanCreateIntakeProposal(IEnumerable<string> roleNames)
    {
        if (IsElevatedReviewer(roleNames))
            return true;

        foreach (var n in roleNames)
        {
            if (string.Equals(n, SystemCatalogRoles.Staff, StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(n, SystemCatalogRoles.NhanVien, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
