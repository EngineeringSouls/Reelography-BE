using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("PhotographerTypes", Schema = "photographer")]
public class PhotographerType: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public required int Id { get; init; }
    
    [StringLength(50)]
    public required string Name { get; init; }
    
    [StringLength(500)]
    public required string Description { get; set; }
    
    public required bool IsActive { get; set; }
}