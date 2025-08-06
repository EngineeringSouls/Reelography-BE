using System.ComponentModel.DataAnnotations;

namespace Reelography.Entities;

/// <summary>
/// Base entity
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedOn { get; set; } 
    [StringLength(500)]
    public string CreatedBy { get; set; } = null!;
    public  DateTime? ModifiedOn { get; set; }
    [StringLength(500)]
    public string? ModifiedBy { get; set; }

}