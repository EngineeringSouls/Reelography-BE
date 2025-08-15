using Reelography.Shared.Enums;

namespace Reelography.Dto.Photographers.Registration.Request;

public sealed class PhotographerRegistrationStateDto
{
    public PhotographerOnboardingStatusEnum OnboardingStatus { get; set; }
    public bool HasPortfolio { get; set; }
    public bool HasPackages { get; set; }
    public bool HasConnectedWithInstagram {get; set; }
}