using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

[Table("PhotographerPackageDetails", Schema = "photographer")]
public class PhotographerPackageDetail: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public required int Id { get; set; }
    
    public required int PhotographerId { get; set; }
    public required int OccasionPackageMappingId { get; set; }
    public string?  Description { get; set; }
    
    [Precision(10,2)]
    public decimal MinPrice { get; set; } 
    public required bool IsActive { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Photographer? Photographer { get; set; }
    
    [ForeignKey(nameof(OccasionPackageMappingId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public required OccasionPackageMapping  OccasionPackageMapping { get; set; }

}