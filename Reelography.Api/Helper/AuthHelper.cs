using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Reelography.Dto.User;
using Reelography.Shared.Enums.User;

namespace Reelography.Api.Helper;


/// <summary>
/// 
/// </summary>
public class AuthHelper
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly string _secret;
    private readonly int    _expiryMinutes;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AuthHelper(IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        _secret        = jwtSection["Secret"]       ?? throw new KeyNotFoundException("JwtSecret");
        _audience      = jwtSection["Audience"]     ?? throw new KeyNotFoundException("Jwt:Audience");
        _issuer        = jwtSection["Issuer"]       ?? throw new KeyNotFoundException("Jwt:Issuer");
        _expiryMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public string GenerateJwtToken(UserClaimDto user)
    {
        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 2) Build claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName,    user.Name)
        };
        foreach (var roleInt in user.Roles)
        {
            // if you have an enum UserRoleEnum { User = 1, Admin = 2, … }
            var roleName = ((UserRoleEnum)roleInt).ToString();
            claims.Add(new Claim(ClaimTypes.Role, roleName));
        }
        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        // 3) Create token descriptor
        var now = DateTime.UtcNow;
        var descriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            NotBefore          = now,
            Expires            = now.AddMinutes(_expiryMinutes),
            Issuer             = _issuer,
            Audience           = _audience,
            SigningCredentials = credentials
        };

        // 4) Create & write token
        var handler = new JwtSecurityTokenHandler();
        var token   = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}