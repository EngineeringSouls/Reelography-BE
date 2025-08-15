namespace Reelography.Shared.Records;

public sealed record InstagramMediaItem(string Id, string MediaType, string MediaUrl, string? ThumbUrl, DateTimeOffset Timestamp, string? Caption, double? SizeInBytes);

