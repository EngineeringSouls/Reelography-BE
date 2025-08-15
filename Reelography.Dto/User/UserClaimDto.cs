namespace Reelography.Dto.User;

/// <summary>
/// User Context
/// </summary>
public class UserClaimDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public List<int> Roles { get; set; } = [];
    public string? Email { get; set; }
}