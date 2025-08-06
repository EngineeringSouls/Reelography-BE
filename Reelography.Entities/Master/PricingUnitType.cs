using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("PricingUnitTypes", Schema = "master")]
public class PricingUnitType: BaseEntity
{
    [Key]
    public required int Id { get; init; }
    [StringLength(50)]
    public required string Code { get; init; }
    public required bool IsActive { get; set; }
    
    public virtual ICollection<OccasionPackageMapping>? OccasionPackageMappings { get; set; }

}