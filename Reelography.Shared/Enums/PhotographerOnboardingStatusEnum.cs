using System.ComponentModel;

namespace Reelography.Shared.Enums;

public enum PhotographerOnboardingStatusEnum
{
    [Description("Basic details Step")]
    BasicDetails = 1,
    
    [Description("Packages Step")]
    PackagesDetails,
    
    [Description("Portfolio Step")]
    PortfolioDetails,
    
    [Description("Draft")]
    Draft,
    
    [Description("Approved")]
    Approved,
    
    [Description("Rejected")]
    Rejected
}