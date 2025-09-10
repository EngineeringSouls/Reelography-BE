using Reelography.Dto.Notification;

namespace Reelography.Notification.Interfaces;

public interface INotificationService
{
    Task<bool> SendOtpNotificationAsync(OtpRequestDto otpRequest, CancellationToken cancellationToken);
    Task<bool> ValidateOtpRequestAsync(OtpValidateRequestDto otpRequest, CancellationToken cancellationToken);
}