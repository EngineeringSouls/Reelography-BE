using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Reelography.Shared.Enums;

namespace Reelography.Entities;

/// <summary>
/// OccasionType class: Wedding, Pre-Wedding etc.
/// </summary>

[Table("OccasionTypes", Schema = "master")]
public class OccasionType: EnumBaseEntity
{
    [Key]
    public required int Id { get; init; }
    public required bool IsActive { get; set; }
    public required bool IsPremium { get; set; }
    
    public virtual ICollection<OccasionPackageMapping>? OccasionPackageMappings { get; set; }
}