namespace Reelography.Shared.Options;

public sealed class BunnyStorageOptions
{
    public string StorageZoneName { get; set; } = string.Empty;      // e.g. "reelography-images"
    public string ApiKey { get; set; } = string.Empty;               // Storage Zone API key
    public string CdnBase { get; set; } = string.Empty;              // e.g. "https://reelo-img.b-cdn.net"
    public string RootPath { get; set; } = "uploads";
}