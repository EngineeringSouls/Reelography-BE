using Reelography.Dto.Master;

namespace Reelography.Service.Contracts.Master;

public interface IOccasionPackageMappingService
{
    Task<IReadOnlyList<OccasionDto>> GetOccasionsAsync(CancellationToken ct);
    Task<IReadOnlyList<PricingUnitDto>> GetPricingUnitsAsync(CancellationToken ct);
    Task<IReadOnlyList<ServicePackagesDto>> GetServicePackagesAsync(CancellationToken ct);
    Task<IReadOnlyList<OccasionPackageMappingDto>> GetOccasionPackageMappingsAsync(CancellationToken ct);
}