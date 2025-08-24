using Reelography.Dto.User;

namespace Reelography.Dto;

/// <summary>
/// User Context Interface
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// JWT Access Token
    /// </summary>
    string? AccessToken { get; }
    /// <summary>
    /// User info
    /// </summary>
    UserClaimDto? User { get;}
}