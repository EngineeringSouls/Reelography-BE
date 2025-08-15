using Reelography.Dto.Photographers.Registration.Request;

namespace Reelography.Dto.Photographers.Registration.Response;

public class PhotographerPackageResponseDto: PhotographerAddPackageRequestDto
{
    public int Id { get; set; }
    public int OccasionId { get; set; }
    public int ServicePackageId { get; set; }
    public int? PricingUnitId { get; set; }
    public decimal MinPrice { get; set; }
    public bool IsActive { get; set; }
}