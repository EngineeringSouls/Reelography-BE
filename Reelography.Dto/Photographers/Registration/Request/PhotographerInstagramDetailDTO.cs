namespace Reelography.Dto.Photographers.Registration.Request;

public class PhotographerInstagramDetailDTO
{
    public required string BusinessAccountId { get; set; }
    public required string LongLivedUserAccessToken { get; set; }
    public required DateTime TokenExpiresOnUtc { get; set; }
    public required DateTime TokenCreatedAt { get; set; }
    public required DateTime TokenLastRefreshedAt { get; set; }
    
    public double FollowersCount { get; set; }
    public double AverageMonthlyViews { get; set; }
}