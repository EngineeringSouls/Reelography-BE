using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reelography.Entities;

public class PhotographerOnboardingStep: EnumBaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]    
    public required int Id { get; init; }
    
    public required bool IsActive { get; set; }
}