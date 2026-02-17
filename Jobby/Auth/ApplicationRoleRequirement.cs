using Microsoft.AspNetCore.Authorization;

namespace Jobby.Auth
{
    public record ApplicationRoleRequirement(params string[] AllowedRoles) : IAuthorizationRequirement;
}
