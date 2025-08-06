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
    public required string IgHandle { get; set; }
    public required string IgToken { get; set; }
    public int IgFollowers { get; set; }
    public int IgMonthlyViews { get; set; }
    
    public DateTime TokenCreatedAt { get; set; }
    public DateTime TokenLastRefreshedAt { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public virtual Photographer? Photographer { get; set; }
}
