using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SmartLunch.Backend.Service.API.Authorization.Role;
using SmartLunch.Backend.Service.API.Authorization.Permission;

namespace SmartLunch.Backend.Service.API.Authorization;

public class DynamicAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public DynamicAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Check if policy already exists
        var existingPolicy = await base.GetPolicyAsync(policyName);
        if (existingPolicy != null)
        {
            return existingPolicy;
        }

        // Parse policy format: "type:value1,value2,value3"
        // Examples: "roles:Admin,User" or "permission:users.create,users.update"
        var separatorIndex = policyName.IndexOf(':');
        if (separatorIndex <= 0 || separatorIndex >= policyName.Length - 1)
        {
            return null;
        }

        var policyType = policyName[..separatorIndex];
        var valuesPart = policyName[(separatorIndex + 1)..];
        var values = valuesPart.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var policyBuilder = new AuthorizationPolicyBuilder();
        policyBuilder.RequireAuthenticatedUser();

        switch (policyType.ToLowerInvariant())
        {
            case "roles":
                policyBuilder.AddRequirements(new RoleRequirement(values));
                break;
            case "permission":
            case "permissions":
                policyBuilder.AddRequirements(new PermissionRequirement(values));
                break;
            default:
                return null;
        }

        return policyBuilder.Build();
    }
}
