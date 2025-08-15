using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Reelography.Shared.Enums;
namespace Reelography.Entities;

[Table("PhotographerPortfolios", Schema = "photographer")]
public class PhotographerPortfolio: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public int Id { get; set; }

    public int PhotographerId { get; set; }
    public int OccasionId { get; set; }
    
    [MaxLength(300)] 
    public string? MediaUrl { get; set; }
    
    
    [MaxLength(300)] 
    public string? ThumbnailUrl { get; set; }
    
    public string? MimeType { get; set; }
    
    public double? SizeBytes { get; set; }
    
    public required int MediaTypeId { get; set; }
    
    public required string StorageAssetId {get;set;}
    
    public required int MediaSourceId { get; set; }
    
    public int? Views { get; set; }
    public int? Likes { get; set; }
    public int? DurationSec { get; set; }
    public bool IsApproved { get; set; } = false;
    
    public string? InstagramMediaId { get; set; }
    
    public string? FileName { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public Photographer Photographer { get; set; } = null!;
    
    [ForeignKey(nameof(OccasionId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public OccasionType Occasion { get; set; } = null!;


}
