using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

[Table("MediaSources", Schema = "master")]
public class MediaSource:  EnumBaseEntity
{
    [Key]
    public required int Id { get; init; }
    
    public required bool IsActive { get; set; }
}