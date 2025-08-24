using Reelography.Api.Helper;
using Reelography.Notification.Interfaces;
using Reelography.Notification.Services;
using Reelography.Service.Contracts.User;
using Reelography.Service.Services.User;

namespace Reelography.Api.Configuration;

public static class ServiceConfiguration
{
    public static void RegisterServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<AuthHelper>();
        RegisterUserServices(serviceCollection);
        RegisterNotificationServices(serviceCollection);
    }

    
    private static void RegisterUserServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUserService, UserService>();
    }

    private static void RegisterNotificationServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<INotificationService, NotificationService>();
        serviceCollection.AddScoped<ITemplateLoaderService, TemplateLoaderService>();
        serviceCollection.AddScoped<IEmailService, EmailService>();
        serviceCollection.AddScoped<ISmsService, SmsService>();
        serviceCollection.AddScoped<IWhatsAppService, WhatsAppService>();

    }
}