using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (!string.IsNullOrEmpty(user.Email))
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

        if (!string.IsNullOrEmpty(user.DeviceId))
            claims.Add(new Claim("device_id", user.DeviceId)); // custom claim

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
        var handler = new JwtSecurityTokenHandler();
        var token   = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

}