using Reelography.Shared.Enums.User;

namespace Reelography.Dto.User;

/// <summary>
/// User Context
/// </summary>
public class UserClaimDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public AuthUserRoleEnum Role { get; set; }
    public string? Email { get; set; }
    public string? DeviceId { get; set; }
}