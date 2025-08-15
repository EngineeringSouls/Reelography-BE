using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

/// <summary>
/// PhotographerGooglePlaceReview
/// </summary>

[Table("PhotographerGooglePlaceReviews", Schema = "photographer")]
public class PhotographerGooglePlaceReview
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public required int Id { get; set; }
    
    public required int GooglePlaceDetailId { get; set; }
    [ForeignKey(nameof(GooglePlaceDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public PhotographerGooglePlaceDetail? GooglePlaceDetail  { get; set; }
    
    public required Guid ReviewId { get; set; }
    public required string ReviewerName  { get; set; }
    public string? ReviewerProfilePicUrl  { get; set; }
    public required decimal StarRating { get; set; }
    public required string ReviewComment { get; set; }
    public DateTime CreateAt{ get; set; }
    public DateTime UpdateAt{ get; set; }
}
