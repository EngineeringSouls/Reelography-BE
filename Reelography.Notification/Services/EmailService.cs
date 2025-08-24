using Reelography.Notification.Interfaces;

namespace Reelography.Notification.Services;

public class EmailService: IEmailService
{
    public async Task<bool> SendOtpNotificationAsync(string email, string otp, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}