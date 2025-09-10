namespace Reelography.Dto.Notification;

public class OtpValidateRequestDto: OtpRequestDto
{
    public string Otp { get; set; } = null!;
}