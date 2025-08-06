using System.ComponentModel;

namespace Reelography.Shared.Enums;

public enum ServicePackageEnum
{
    [Description("Bronze")]
    Bronze = 1,
    
    [Description("Silver")]
    Silver,
    
    [Description("Gold")]
    Gold,
    
    [Description("Platinum")]
    Platinum,
    
    [Description("Vibranium")]
    Vibranium,
    
    [Description("Special")]
    Special=99,
    
    [Description("None")]
    None=100
}