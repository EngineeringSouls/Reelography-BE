using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

/// <summary>
/// PhotographerInstagramDetail
/// </summary>

[Table("PhotographerInstagramDetails", Schema = "photographer")]
public class PhotographerInstagramDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public int Id { get; set; }
    
    public required int PhotographerId { get; set; }
    public required string BusinessAccountId { get; set; }
    public required string LongLivedUserAccessToken { get; set; }
    public required DateTime TokenExpiresOnUtc { get; set; }
    public required DateTime TokenCreatedAt { get; set; }
    public required DateTime TokenLastRefreshedAt { get; set; }
    
    public double FollowersCount { get; set; }
    public double AverageMonthlyViews { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Photographer? Photographer { get; set; }
}
