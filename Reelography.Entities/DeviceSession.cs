namespace Reelography.Entities;

public class DeviceSession
{
    public Guid Id { get; set; }              // sid
    public int AuthUserId { get; set; }      // maps to your Identity user PK (Guid)
    public string? DeviceId { get; set; }     // your client-generated device ID (optional)
    public string? UserAgentHash { get; set; }
    public string? Platform { get; set; }     // "web", "ios", "android"
    public string? IpAddress { get; set; }    // optional; don’t hard-pin to IP
    public string RefreshTokenHash { get; set; } = null!;
    public DateTime RefreshTokenExpiresOn { get; set; }
    public bool Revoked { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastSeenOn { get; set; }
}