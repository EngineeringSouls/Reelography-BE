using Reelography.Dto.Photographers.Registration.Request;

namespace Reelography.Service.Contracts.Photographer;

public interface IPhotographerRegistrationService
{
    Task<PhotographerRegistrationStateDto> GetCurrentStateAsync(int appUserId, CancellationToken ct);
    Task<object> UpsertBasicDetailsAsync(int appUserId, PhotographerBasicDetailsRequestDto detailsRequestDto, CancellationToken ct);
    Task<object> AddPackagesAsync(int appUserId, PhotographerAddPackagesListRequestDto dto, CancellationToken ct);
    Task<object> AddPortfolioAsync(int appUserId, PhotographerAddPortfolioListItemsRequestDto dto, CancellationToken ct);
    Task<object> CompleteAsync(int appUserId, CancellationToken ct);
}
