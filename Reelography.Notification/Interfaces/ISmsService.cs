namespace Reelography.Notification.Interfaces;

public interface ISmsService
{
    Task<bool> SendOtpNotificationAsync(string mobile,  string otp,CancellationToken cancellationToken);
}