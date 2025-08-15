namespace Reelography.Shared.Records;

public sealed record InstagramUserProfileWithMedia(string IgUserId, string Username, double FollowersCount, List<InstagramMediaItem> MediaItems, double MonthlyAverageViews);