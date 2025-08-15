using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Reelography.Shared.Enums;

namespace Reelography.Entities;

[Table("Photographers", Schema = "photographer")]
public class Photographer: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    
    public required int AppUserId { get; set; }
    
    [MaxLength(50)] public required string Name { get; set; }
    
    [MaxLength(300)] public string? ProfileImageUrl { get; set; }
    
    [MaxLength(50)] public required string StudioName { get; set; }
    
    public int YearsExperience { get; set; }

    /// <summary>
    /// Freelancer, Professional
    /// </summary>
    public required int PhotographerTypeId { get; set; }
    [ForeignKey(nameof(PhotographerTypeId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public required PhotographerType PhotographerType { get; set; }
    
    public int InstagramDetailId { get; set; }
    [ForeignKey(nameof(InstagramDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public PhotographerInstagramDetail? InstagramDetail { get; set; }

    public required int GooglePlaceDetailId { get; set; }
    [ForeignKey(nameof(GooglePlaceDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public PhotographerGooglePlaceDetail? GooglePlaceDetail { get; set; }

    public ICollection<PhotographerInAppReview>? Reviews { get; set; }
    public ICollection<PhotographerPackageDetail>? PackageDetails { get; set; }
    public ICollection<PhotographerPortfolio>? Portfolio { get; set; }
    public ICollection<FavouritePhotographer>? FavouriteBy { get; set; }
}