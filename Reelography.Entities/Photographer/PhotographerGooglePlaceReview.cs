using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

/// <summary>
/// PhotographerGooglePlaceReview
/// </summary>

[Table("PhotographerGooglePlaceReviews", Schema = "photographer")]
public class PhotographerGooglePlaceReview: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public required int Id { get; set; }
    
    public required int PhotographerGooglePlaceDetailId { get; set; }
    [ForeignKey(nameof(PhotographerGooglePlaceDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public PhotographerGooglePlaceDetail? PhotographerGooglePlaceDetail  { get; set; }
    
    public required string ReviewerName  { get; set; }
    public string? ReviewerProfilePicUrl  { get; set; }
    public required decimal StarRating { get; set; }
    public required string ReviewComment { get; set; }
}
