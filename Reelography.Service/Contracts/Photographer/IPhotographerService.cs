using Reelography.Dto.Master;
using Reelography.Dto.Photographers.Registration.Request;
using Reelography.Dto.Photographers.Registration.Response;
using Reelography.Entities;

namespace Reelography.Service.Contracts.Photographer;

public interface IPhotographerService
{
    Task<PhotographerBasicDetailsResponseDto?> GetBasicAsync(int appUserId, CancellationToken ct);
    Task<List<PhotographerPackageResponseDto>> GetPackagesAsync(int appUserId, CancellationToken ct);
    Task<List<PhotographerPortfolioItemResponseDto>> GetPortfolioAsync(int appUserId, CancellationToken ct);
    
    Task<PhotographerInstagramDetailDTO?> GetInstagramDetailAsync(int appUserId, CancellationToken ct);
    Task<List<OccasionDto>> GetPhotographerOccasionsAsync(int appUserId, CancellationToken ct);
    
    Task AddPhotographerInstagramBasicDetailsAsync(int appUserId, PhotographerInstagramDetailDTO instagramDetail, CancellationToken ct);

}