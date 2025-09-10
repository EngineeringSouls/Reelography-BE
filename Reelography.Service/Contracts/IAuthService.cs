namespace Reelography.Service.Contracts;

public interface IAuthService
{
    Task<bool> InsertRefreshTokenAsync(string refreshToken, int userId, string deviceId);

    Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId, string deviceId);

    Task<bool> RevokeRefreshTokenAsync(string refreshToken, int userId, string deviceId);
}
