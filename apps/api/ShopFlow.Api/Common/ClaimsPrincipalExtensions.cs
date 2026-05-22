using System.Security.Claims;
using ShopFlow.Api.Common;

namespace ShopFlow.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var guid))
            throw new ForbiddenException("Token sem identificador de usuário válido");
        return guid;
    }

    public static bool IsAdmin(this ClaimsPrincipal principal) =>
        principal.IsInRole("Admin");
}
