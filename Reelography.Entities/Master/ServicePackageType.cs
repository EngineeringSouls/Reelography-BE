using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("ServicePackageTypes", Schema = "master")]
public class ServicePackageType: BaseEntity
{
    [Key]
    public required int Id { get; init; }
    
    [StringLength(50)]
    public required string Name { get; init; }
    
    [StringLength(1024)]
    public required string Description { get; set; }
    
    public required bool IsActive { get; set; }
    
    public virtual ICollection<OccasionPackageMapping>? OccasionPackageMappings { get; set; }
}