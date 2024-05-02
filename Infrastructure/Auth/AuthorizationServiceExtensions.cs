using Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Shared.Authorization;
using System.Security.Claims;

namespace Infrastructure.Auth;
public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource) =>
        (await service.AuthorizeAsync(user, null, Permission.NameFor(action, resource))).Succeeded;
}