using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reelography.Entities;

[Table("FavouritePhotographers", Schema = "photographer")]
public class FavouritePhotographer: BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    
    public required int UserId { get; set; }
    public required int PhotographerId { get; set; }
    
    public DateTime AddedAt { get; set; }
    
    [ForeignKey(nameof(PhotographerId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public Photographer Photographer { get; set; } = null!;
}