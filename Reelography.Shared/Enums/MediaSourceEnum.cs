using System.ComponentModel;

namespace Reelography.Shared.Enums;

public enum MediaSourceEnum
{
    [Description("Synced from Instagram")]
    Instagram = 1,
    
    [Description("Manually uploaded")]
    Manual
}