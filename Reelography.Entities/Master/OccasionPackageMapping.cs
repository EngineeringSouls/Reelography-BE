using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("OccasionPackageMappings", Schema = "master")]
public class OccasionPackageMapping: BaseEntity
{
    [Key]
    public required int Id { get; init; }    
    
    public int OccasionId { get; set; }
    public OccasionType? OccasionType { get; set; }

    public int? ServicePackageTypeId { get; set; }
    public ServicePackageType? ServicePackageType { get; set; }

    public int PricingUnitId { get; set; }
    public PricingUnitType? PricingUnitType { get; set; }
}