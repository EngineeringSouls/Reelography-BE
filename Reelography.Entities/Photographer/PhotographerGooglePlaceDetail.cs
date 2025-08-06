using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

/// <summary>
/// PhotographerGooglePlaceDetail
/// </summary>

[Table("PhotographerGooglePlaceDetails", Schema = "photographer")]
public class PhotographerGooglePlaceDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    
    public required int PhotographerId { get; set; }
    
    public required int GooglePlaceId { get; set; }
    
    [MaxLength(120)] public required string LocationName { get; set; }
    [MaxLength(120)] public required string StreetName { get; set; }
    [MaxLength(120)] public required string City { get; set; }
    [MaxLength(120)] public required string District { get; set; }
    [MaxLength(120)] public required string State { get; set; }

    public double Lat { get; set; }
    public double Lang { get; set; }
    
    public decimal AverageRating { get; set; }
    public int TotalReviewCount { get; set; }
    
    public ICollection<PhotographerGooglePlaceReview>? PhotographerGoogleReviews { get; set; }
}
