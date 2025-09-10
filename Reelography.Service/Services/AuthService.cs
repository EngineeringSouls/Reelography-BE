using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reelography.Data;
using Reelography.Entities;
using Reelography.Service.Contracts;

namespace Reelography.Service.Services;

public class AuthService(
    ReelographyDbContext dbContext,
    IPasswordHasher<DeviceSession> hasher,
    IConfiguration configuration): IAuthService
{
    public async Task<bool> InsertRefreshTokenAsync(string refreshToken, int userId, string deviceId)
    {
        var session = await dbContext.DeviceSessions
            .FirstOrDefaultAsync(s => s.AuthUserId == userId && s.DeviceId == deviceId);
        var jwtSection = configuration.GetSection("Jwt");
        int.TryParse(jwtSection["refreshTokenExpiryInDays"], out var refreshTokenExpiryInDays);
        if (session == null)
        {
            session = new DeviceSession
            {
                Id = Guid.NewGuid(),
                AuthUserId = userId,
                DeviceId = deviceId,
                RefreshTokenHash = hasher.HashPassword(null!, refreshToken),
                RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpiryInDays),
                CreatedOn = DateTime.UtcNow,
                LastSeenOn = DateTime.UtcNow,
                Revoked = false
            };
            dbContext.DeviceSessions.Add(session);
        }
        else
        {
            session.RefreshTokenHash = hasher.HashPassword(session, refreshToken);
            session.RefreshTokenExpiresOn = DateTime.UtcNow.AddDays(refreshTokenExpiryInDays);
            session.LastSeenOn = DateTime.UtcNow;
            session.Revoked = false;
            dbContext.DeviceSessions.Update(session);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string refreshToken, int userId, string deviceId)
    {
        var session = await dbContext.DeviceSessions
            .FirstOrDefaultAsync(s => s.AuthUserId == userId && s.DeviceId == deviceId);

        if (session == null || session.Revoked || session.RefreshTokenExpiresOn < DateTime.UtcNow)
            return false;

        var result = hasher.VerifyHashedPassword(session, session.RefreshTokenHash, refreshToken);
        if (result == PasswordVerificationResult.Failed)
            return false;

        session.LastSeenOn = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, int userId, string deviceId)
    {
        var session = await dbContext.DeviceSessions
            .FirstOrDefaultAsync(s =>
                s.AuthUserId == userId &&
                s.DeviceId == deviceId &&
                !s.Revoked &&
                s.RefreshTokenExpiresOn > DateTime.UtcNow);

        if (session == null)
            return false;

        // Verify the refresh token against the stored hash
        var result = hasher.VerifyHashedPassword(null!, session.RefreshTokenHash, refreshToken);
        if (result == PasswordVerificationResult.Failed)
            return false;

        session.Revoked = true;
        session.LastSeenOn = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        return true;
    }
}