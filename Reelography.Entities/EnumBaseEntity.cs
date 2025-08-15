using System.ComponentModel.DataAnnotations;

namespace Reelography.Entities;

public class EnumBaseEntity: BaseEntity
{
    [StringLength(50)]
    public required string Name { get; init; }
    
    [StringLength(50)]
    public required string Description { get; set; }
}