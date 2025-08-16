namespace Reelography.Dto.User;

/// <summary>
/// User Context
/// </summary>
public class UserClaimDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<int> Roles { get; set; } = [];
    public string? Email { get; set; }
}