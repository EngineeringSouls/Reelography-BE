using Microsoft.AspNetCore.Http.Features;
using Reelography.Api.Helper;
using Reelography.Notification.Interfaces;
using Reelography.Notification.Services;
using Reelography.Service.Contracts.Master;
using Reelography.Service.Contracts.Photographer;
using Reelography.Service.Contracts.User;
using Reelography.Service.External;
using Reelography.Service.External.Contracts;
using Reelography.Service.Master;
using Reelography.Service.Photographer;
using Reelography.Service.Services.User;
using Reelography.Shared.Options;

namespace Reelography.Api.Configuration;

/// <summary>
/// Service Configuration Extension class
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Extension method to register the services
    /// </summary>
    /// <param name="builder"></param>
    public static void RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<AuthHelper>();
        RegisterGooglePlacesServices(builder);
        RegisterBunnyStorageServices(builder);
        RegisterIntsaServices(builder);
        RegisterPhotographerServices(builder);
        
        RegisterUserServices(builder);
        RegisterNotificationServices(builder);

        builder.Services.Configure<FormOptions>(o =>
        {
            o.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; // 2 GB
        });
        
        builder.Services.AddOptions<InstagramOptions>()
            .Bind(builder.Configuration.GetSection("Instagram"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "Instagram ClientId missing")
            .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "Instagram ClientSecret missing")
            .Validate(o => !string.IsNullOrWhiteSpace(o.RedirectUri), "Instagram RedirectUri missing")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Scopes), "Instagram Scopes missing")
            .ValidateOnStart();
        
        builder.Services.AddOptions<OAuthOptions>()
            .Bind(builder.Configuration.GetSection("OAuth"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.StateSecret), "StateSecret missing")
            .ValidateOnStart();
    }

    
    private static void RegisterUserServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserService, UserService>();
    }

    private static void RegisterGooglePlacesServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<GooglePlacesOptions>(builder.Configuration.GetSection("GooglePlaces"));
        builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();
    }
    private static void RegisterBunnyStorageServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<BunnyStorageOptions>(builder.Configuration.GetSection("BunnyStorage"));
        builder.Services.Configure<BunnyStreamOptions>(builder.Configuration.GetSection("BunnyStream"));
        builder.Services.AddHttpClient<IStorageService, BunnyHybridStorageService>();
    }
    private static void RegisterIntsaServices(WebApplicationBuilder builder)
    {
        builder.Services.Configure<InstagramOptions>(builder.Configuration.GetSection("Instagram"));
        builder.Services.AddScoped<IInstagramService, InstagramService>();
    }
    private static void RegisterPhotographerServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPhotographerRegistrationService, PhotographerRegistrationService>();
        builder.Services.AddScoped<IOccasionPackageMappingService, OccasionPackageMappingService>();
        builder.Services.AddScoped<IPhotographerService, PhotographerService>();
    }
    private static void RegisterNotificationServices(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<ITemplateLoaderService, TemplateLoaderService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ISmsService, SmsService>();
        builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();

    }
}