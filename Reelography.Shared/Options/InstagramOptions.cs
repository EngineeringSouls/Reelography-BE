namespace Reelography.Shared.Options;

public sealed class InstagramOptions
{
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;
    public string Scopes { get; set; } = "instagram_basic";
    public string ApiVersion { get; set; } = "v20.0";
    public int DefaultMediaLimit { get; set; } = 12;
}