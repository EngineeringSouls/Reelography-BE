using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

/// <summary>
/// PhotographerReview
/// </summary>

[Table("PhotographerInAppReviews", Schema = "photographer")]
public class PhotographerInAppReview: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public int Id { get; set; }
    
    public required int PhotographerId { get; set; }
    public required int UserId { get; set; }
    public required int BookingId { get; set; }
    
    [MaxLength(500)] 
    public required string ReviewComment { get; set; }
    
    [Precision(2,1)]
    [Range(1,5)]
    public required decimal StarRating { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Photographer? Photographer { get; set; }
}
