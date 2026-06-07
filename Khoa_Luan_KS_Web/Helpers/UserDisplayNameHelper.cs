using System.Security.Claims;

namespace Khoa_Luan_KS_Web.Helpers;

public static class UserDisplayNameHelper
{
    public static string Resolve(string? fullName, string? email, string? username = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            return fullName.Trim();

        var candidate = !string.IsNullOrWhiteSpace(username) ? username.Trim() : email?.Trim();
        if (string.IsNullOrWhiteSpace(candidate))
            return "Khách hàng";

        if (candidate.Contains('@'))
            return candidate.Split('@')[0];

        return candidate;
    }

    public static string FromPrincipal(ClaimsPrincipal? user, ISession? session)
    {
        var fullName = user?.FindFirst("FullName")?.Value;
        var sessionName = session?.GetString("user_name");
        var nameClaim = user?.FindFirst(ClaimTypes.Name)?.Value;
        var email = user?.FindFirst(ClaimTypes.Email)?.Value ?? session?.GetString("user_email");

        if (!string.IsNullOrWhiteSpace(fullName))
            return fullName.Trim();

        if (!string.IsNullOrWhiteSpace(sessionName) && !sessionName.Contains('@'))
            return sessionName.Trim();

        if (!string.IsNullOrWhiteSpace(nameClaim) && !nameClaim.Contains('@'))
            return nameClaim.Trim();

        return Resolve(null, email, nameClaim);
    }
}
