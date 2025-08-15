namespace Reelography.Shared.Records;

public sealed record StoragePutResult(string Provider, string AssetId, string Url, string? ThumbUrl);
