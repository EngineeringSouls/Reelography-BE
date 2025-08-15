using System.ComponentModel.DataAnnotations;

namespace Reelography.Dto.Photographers.Registration.Request;

public class PhotographerAddPackageRequestDto
{
    public required int OccasionPackageMappingId { get; set; }
    [MaxLength(140)] public string? Title { get; set; }
    [Range(0, int.MaxValue)] public int BasePriceInr { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
}