using System.ComponentModel;

namespace Reelography.Shared.Enums.User;

/// <summary>
/// User Role Enums
/// </summary>
public enum AuthUserRoleEnum
{
    /// <summary>
    /// Admin
    /// </summary>
    [Description("Admin")]
    Admin = 1,
    
    /// <summary>
    /// User
    /// </summary>
    [Description("User")]
    User,
    
    /// <summary>
    /// Photographer
    /// </summary>
    [Description("Photographer")]
    Photographer
}