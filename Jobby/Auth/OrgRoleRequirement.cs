using Microsoft.AspNetCore.Authorization;

namespace Jobby.Auth
{
    public record OrgRoleRequirement(params string[] AllowedRoles)
    : IAuthorizationRequirement;
}
