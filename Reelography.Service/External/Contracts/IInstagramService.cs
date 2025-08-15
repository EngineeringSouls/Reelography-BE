using Reelography.Shared.Records;

namespace Reelography.Service.External.Contracts;

public interface IInstagramService
{
    string CreateState(int authUserId);
    (bool validateSuccess, int authUserId) ValidateState(string state);
    string BuildAuthorizationUrl(string state);
    Task<(string? longToken, double? expiresInSec)> ExchangeCodeAsync(string code, CancellationToken ct);
    Task<InstagramUserProfileWithMedia> GetUserProfileWithMediaAsync(string accessToken, CancellationToken ct);
}