using System.ComponentModel;

namespace Reelography.Shared.Enums;

/// <summary>
/// PricingUnitEnum
/// </summary>
public enum PricingUnitEnum
{
    [Description("Per Hour")]
    PerHour = 1,
    
    [Description("Per Day")]
    PerDay,
    
    [Description("Per Shoot")]
    PerShoot,
    
    [Description("Per Package")]
    PerPackage
}