using Microsoft.Extensions.Configuration;
using Reelography.Dto.Notification;
using Reelography.Notification.Interfaces;
using Reelography.Shared.Constants;
using Reelography.Shared.StaticHelpers;
using Twilio.Rest.Verify.V2.Service;

namespace Reelography.Notification.Services;

public class NotificationService(
    ISmsService smsService,
    IEmailService emailService,
    IWhatsAppService whatsAppService,
    IConfiguration configuration)
    : NotificationServiceBase(configuration), INotificationService
{
    private readonly IWhatsAppService _whatsAppService = whatsAppService;

    public async Task<bool> SendOtpNotificationAsync(OtpRequestDto otpRequest, CancellationToken cancellationToken)
    {
        string otp = NotificationHelper.GenerateOtp();
        if (otpRequest.Mobile != null)
        {
            await smsService.SendOtpNotificationAsync(otpRequest.Mobile, otp, cancellationToken);
        }
        else if (otpRequest.Email != null)
        {
            await emailService.SendOtpNotificationAsync(otpRequest.Email, otp, cancellationToken);
        }
        else
        {
            throw new ArgumentException("You must provide a Mobile or Email address to SendOtpNotification");
        }
        return true;
    }

    public async Task<bool> ValidateOtpRequestAsync(OtpValidateRequestDto otpRequest, CancellationToken cancellationToken)
    {
        var verifySid = TwilioConfigurationSection["VerifySid"] ?? throw new KeyNotFoundException(string.Format(ErrorMessages.KeyNotFound,"Verify Sid"));
        var verification = await VerificationCheckResource.CreateAsync(
            to: otpRequest.Mobile ?? otpRequest.Email,
            code:otpRequest.Otp,
            pathServiceSid: verifySid
        );
        return verification is { Status: "approved" };
    }
}   