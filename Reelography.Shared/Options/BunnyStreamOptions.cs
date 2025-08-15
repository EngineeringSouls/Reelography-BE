namespace Reelography.Shared.Options;

public class BunnyStreamOptions
{
    public string LibraryId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string CdnBase { get; set; } = "https://vz-{{LIB}}.b-cdn.net"; // e.g. vz-123456.b-cdn.net
}