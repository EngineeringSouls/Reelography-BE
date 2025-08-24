using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Reelography.Dto.User;
using Reelography.Shared.Enums.User;

namespace Reelography.Dto;


public class HttpUserContext(IHttpContextAccessor http) : IUserContext
{
    public string? AccessToken
    {
        get
        {
            var header = http.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(header) ||
                !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            return header["Bearer ".Length..].Trim();
        }
    }

    public UserClaimDto? User
    {
        get
        {
            var user = http.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return null;

            // Pull claims via LINQ—no special extensions needed:
            var idClaim    = user.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                ?.Value;
            var emailClaim = user.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
            var roleClaim = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c =>
                {
                    if (Enum.TryParse<AuthUserRoleEnum>(c.Value, out var parsed))
                        return (int)parsed;
                    return (int?)null;
                })
                .Where(r => r.HasValue)
                .Select(r => r!.Value)
                .FirstOrDefault();

            return new UserClaimDto
            {
                Id    = int.TryParse(idClaim!, out var id) ? id : 0,
                Email = emailClaim!,
                Role = (AuthUserRoleEnum)roleClaim
            };
        }
    }
}