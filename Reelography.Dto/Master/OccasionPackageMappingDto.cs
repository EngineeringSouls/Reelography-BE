using Reelography.Shared.Enums;

namespace Reelography.Dto.Master;

public class OccasionPackageMappingDto
{
    public required int Id { get; set; }
    public required OccasionEnum OccasionTypeId { get; set; }
    public required ServicePackageEnum ServicePackageId { get; set; }
    
    public required int PricingUnitId { get; set; }

}