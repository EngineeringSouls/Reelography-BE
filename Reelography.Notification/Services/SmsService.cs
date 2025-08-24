using Microsoft.Extensions.Configuration;
using Reelography.Notification.Interfaces;
using Reelography.Shared.Constants;
using Reelography.Shared.StaticHelpers;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Reelography.Notification.Services;

public class SmsService(ITemplateLoaderService templateLoaderService, IConfiguration configuration)
    : NotificationServiceBase(configuration), ISmsService
{
    public async Task<bool> SendOtpNotificationAsync(string mobile,  string otp, CancellationToken cancellationToken)
    {
        var serviceSid = TwilioConfigurationSection["VerifySid"];
        if (string.IsNullOrEmpty(serviceSid))
        {
            throw new InvalidOperationException("VerifyServiceSid is not configured in Twilio settings.");
        }

        var verification = await Twilio.Rest.Verify.V2.Service.VerificationResource.CreateAsync(
            to: mobile,
            channel: "sms",
            pathServiceSid: serviceSid
        );
        return verification.Status == "pending";
    }
}