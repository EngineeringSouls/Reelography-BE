using System.ComponentModel;
namespace Reelography.Shared.Enums;

/// <summary>
/// Enum for Occasion
/// </summary>
public enum OccasionEnum
{
    [Description("Wedding")]
    Wedding = 1,
    
    [Description("Pre-Wedding")]
    PreWedding,
    
    [Description("Birthday")]
    Birthday
}