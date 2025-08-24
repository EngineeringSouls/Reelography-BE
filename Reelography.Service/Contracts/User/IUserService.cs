using Reelography.Dto.User;

namespace Reelography.Service.Contracts.User;

/// <summary>
/// Provides methods to retrieve user information.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Retrieves user information based on the user's ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The user details if found, otherwise null.</returns>
    Task<UserClaimDto> GetUserClaimDto(int userId, CancellationToken cancellationToken = default);
}