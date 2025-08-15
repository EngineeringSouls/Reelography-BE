using System.ComponentModel.DataAnnotations;

namespace Reelography.Entities;

/// <summary>
/// Base entity
/// </summary>
public abstract class BaseEntity
{
    public required DateTime CreatedOn { get; set; } 
    [StringLength(500)]
    public required string CreatedBy { get; set; }
    public  DateTime? ModifiedOn { get; set; }
    [StringLength(500)]
    public string? ModifiedBy { get; set; }

}