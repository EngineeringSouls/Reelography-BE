using Microsoft.Extensions.Configuration;
using Reelography.Shared.Constants;
using Twilio;

namespace Reelography.Notification;

public abstract class NotificationServiceBase
{
    protected readonly IConfigurationSection TwilioConfigurationSection;
    protected NotificationServiceBase(IConfiguration configuration)
    {
        TwilioConfigurationSection = configuration.GetSection("Twilio"); 
        string accountSid = TwilioConfigurationSection["AccountSid"]?? throw new KeyNotFoundException(string.Format(ErrorMessages.KeyNotFound, "Account SID"));
        string authToken = TwilioConfigurationSection["AuthToken"]?? throw new KeyNotFoundException(string.Format(ErrorMessages.KeyNotFound, "Auth Token"));
        TwilioClient.Init(accountSid, authToken);
    }
}