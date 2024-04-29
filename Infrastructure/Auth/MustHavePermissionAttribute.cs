using Authorization;
using Microsoft.AspNetCore.Authorization;
using Shared.Authorization;

namespace Infrastructure.Auth;
public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = Permission.NameFor(action, resource);
}