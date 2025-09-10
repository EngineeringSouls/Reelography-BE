namespace Reelography.Notification.Interfaces;

public interface IEmailService
{
    Task<bool> SendOtpNotificationAsync(string email, string otp, CancellationToken cancellationToken);
}