using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

/// <summary>
/// PhotographerGooglePlaceDetail
/// </summary>

[Table("PhotographerGooglePlaceDetails", Schema = "photographer")]
public class PhotographerGooglePlaceDetail: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    
    public required int PhotographerId { get; set; }
    
    public required string GooglePlaceId { get; set; }
    
    public required string FormattedAddress { get; set; }
    
    [MaxLength(120)] public string? Line1 { get; set; }
    [MaxLength(120)] public string? Line2 { get; set; }
    [MaxLength(80)] public string? City { get; set; }
    [MaxLength(80)] public string? State { get; set; }
    [MaxLength(80)] public string? Country { get; set; }
    [MaxLength(20)] public string? Postal { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    
    public decimal AverageRating { get; set; }
    public int TotalReviewCount { get; set; }
    
    [ForeignKey(nameof(PhotographerId))]
    public virtual Photographer? Photographer { get; set; }
    public ICollection<PhotographerGooglePlaceReview>? PhotographerGoogleReviews { get; set; }
}
