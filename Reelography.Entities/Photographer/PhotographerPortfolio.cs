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
    
    public required int MediaTypeId { get; set; }
    public required int MediaSourceId { get; set; }
    
    public int? Views { get; set; }
    public int? Likes { get; set; }
    public short? DurationSec { get; set; }
    public bool IsApproved { get; set; } = false;
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public Photographer Photographer { get; set; } = null!;
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public OccasionType Occasion { get; set; } = null!;


}
