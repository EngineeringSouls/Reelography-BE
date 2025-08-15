using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("MediaTypes", Schema = "master")]
public class MediaType: EnumBaseEntity
{
    [Key]
    public required int Id { get; init; }
    
    public required bool IsActive { get; set; }
}