using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.ExceptionServices;
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
    public string? ProfileImageStorageId {get; set;}

    [MaxLength(100)] public required string StudioName { get; set; }
    [MaxLength(250)] public required string AboutInfo { get; set; }

    public required bool IsFeatured { get; set; } = false;
    
    public required int YearsExperience { get; set; }
    
    [Precision(3,1)]
    [Range(0, 100)]
    public decimal? TrustScore { get; set; }

    /// <summary>
    /// Freelancer, Professional
    /// </summary>
    public required int PhotographerTypeId { get; set; }
    public int? InstagramDetailId { get; set; }
    
    public int? GooglePlaceDetailId { get; set; }
    
    public int? OnboardingStepId { get; set; }
    
    
    [ForeignKey(nameof(OnboardingStepId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual PhotographerOnboardingStep? OnboardingStep { get; set; }

    [ForeignKey(nameof(PhotographerTypeId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual PhotographerType? PhotographerType { get; set; }

    [ForeignKey(nameof(InstagramDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual PhotographerInstagramDetail? InstagramDetail { get; set; }
    
    [ForeignKey(nameof(GooglePlaceDetailId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual PhotographerGooglePlaceDetail? GooglePlaceDetail { get; set; }
    
    public virtual ICollection<PhotographerInAppReview>? Reviews { get; set; }
    public virtual ICollection<PhotographerPackageDetail>? PackageDetails { get; set; }
    public virtual ICollection<PhotographerPortfolio>? PortfolioDetails { get; set; }
    public virtual ICollection<FavouritePhotographer>? FavouriteBy { get; set; }
}