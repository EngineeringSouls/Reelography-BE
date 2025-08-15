using Microsoft.EntityFrameworkCore;
using Reelography.Data;
using Reelography.Dto.Master;
using Reelography.Dto.Photographers.Registration.Request;
using Reelography.Dto.Photographers.Registration.Response;
using Reelography.Entities;
using Reelography.Service.Contracts.Photographer;
using Reelography.Shared.Enums;

namespace Reelography.Service.Photographer;

public class PhotographerService: IPhotographerService
{
    private readonly ReelographyDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public PhotographerService(ReelographyDbContext dbContext)
    {
        _dbContext = dbContext;
    }    
    
    public async Task<PhotographerBasicDetailsResponseDto?> GetBasicAsync(int appUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .AsNoTracking()
            .Include(x => x.GooglePlaceDetail)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);

        if (p is null) return null;

        return new PhotographerBasicDetailsResponseDto
        {
            Name = p.Name,
            StudioName = p.StudioName,
            AboutInfo = p.AboutInfo,
            PhotographerTypeId = (PhotographerTypeEnum)p.PhotographerTypeId,
            YearsExperience = p.YearsExperience,
            GooglePlaceId = p.GooglePlaceDetail?.GooglePlaceId,
            AddressLine1 = p.GooglePlaceDetail?.Line1,
            AddressLine2 = p.GooglePlaceDetail?.Line2,
            City = p.GooglePlaceDetail?.City,
            State = p.GooglePlaceDetail?.State,
            Country = p.GooglePlaceDetail?.Country,
            Postal = p.GooglePlaceDetail?.Postal,
            Lat = p.GooglePlaceDetail?.Lat,
            Lng = p.GooglePlaceDetail?.Lng,
            AverageRating = p.GooglePlaceDetail?.AverageRating,
            TotalReviewCount = p.GooglePlaceDetail?.TotalReviewCount,
            ProfileImageUrl = p.ProfileImageUrl,
            FormattedAddress = p.GooglePlaceDetail?.FormattedAddress
        };
    }

    public async Task<List<PhotographerPackageResponseDto>> GetPackagesAsync(int appUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);
        if (p is null) return [];

        var query =
            from pkg in _dbContext.PhotographerPackageDetails.AsNoTracking()
            join map in _dbContext.OccasionPackageMappings.AsNoTracking()
                on pkg.OccasionPackageMappingId equals map.Id
            where pkg.PhotographerId == p.Id
            select new PhotographerPackageResponseDto
            {
                Id = pkg.Id,
                Title = pkg.Title,
                OccasionPackageMappingId = pkg.OccasionPackageMappingId,
                OccasionId = map.OccasionId,
                ServicePackageId = map.ServicePackageTypeId ?? (int) ServicePackageEnum.None,
                PricingUnitId = map.PricingUnitId,
                MinPrice = pkg.MinPrice,
                Description = pkg.Description,
                IsActive = pkg.IsActive
            };

        return await query.ToListAsync(ct);
    }

    public async Task<List<PhotographerPortfolioItemResponseDto>> GetPortfolioAsync(int appUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);
        if (p is null) return [];

        return await _dbContext.PhotographerPortfolios
            .AsNoTracking()
            .Where(x => x.PhotographerId == p.Id)
            .OrderByDescending(x => x.Id)
            .Select(x => new PhotographerPortfolioItemResponseDto
            {
                Id = x.Id,
                OccasionId = x.OccasionId,
                MediaUrl = x.MediaUrl ?? string.Empty,
                ThumbnailUrl = x.ThumbnailUrl,
                Type = (MediaTypeEnum)x.MediaTypeId,
                Source = (MediaSourceEnum)x.MediaSourceId,
                InstagramMediaId = x.InstagramMediaId,
                FileName = x.FileName,
                SizeInBytes = x.SizeBytes,
                MimeType = x.MimeType
            })
            .ToListAsync(ct);
    }

    public async Task<PhotographerInstagramDetailDTO?> GetInstagramDetailAsync(int authUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppUserId == authUserId, ct) ??
                throw new KeyNotFoundException($"Photographer with authUserId: {authUserId} not found");
        

        return await _dbContext.PhotographerInstagramDetails
            .AsNoTracking()
            .Where(x => x.PhotographerId == p.Id)
            .OrderByDescending(x => x.Id)
            .Select(x => new PhotographerInstagramDetailDTO
            {
                BusinessAccountId = x.BusinessAccountId,
                LongLivedUserAccessToken = x.LongLivedUserAccessToken,
                TokenExpiresOnUtc = x.TokenExpiresOnUtc,
                TokenCreatedAt = x.TokenCreatedAt,
                TokenLastRefreshedAt = x.TokenLastRefreshedAt,
                FollowersCount = x.FollowersCount,
                AverageMonthlyViews = x.AverageMonthlyViews
            })
            .FirstOrDefaultAsync(ct);
    }

    
    public async Task<List<OccasionDto>> GetPhotographerOccasionsAsync(int appUserId, CancellationToken ct)
    {
        var photographerOccasions = await _dbContext.Photographers
            .Include(x => x.PackageDetails!)
            .ThenInclude(x => x.OccasionPackageMapping)
            .ThenInclude(x => x!.OccasionType)
            .Where(x => x.AppUserId == appUserId && x.PackageDetails != null && x.PackageDetails.Any())
            .SelectMany(y=>y.PackageDetails!)
            .Select(x=> new OccasionDto()
            {
                Id = x.OccasionPackageMapping!.OccasionId,
                Name = x.OccasionPackageMapping!.OccasionType!.Name,
                Description = x.OccasionPackageMapping!.OccasionType!.Description
            })
            .AsNoTracking()
            .Distinct()
            .ToListAsync(cancellationToken: ct);
        
        return photographerOccasions;
    }

    public async Task AddPhotographerInstagramBasicDetailsAsync(int authUserId, PhotographerInstagramDetailDTO instagramDetail,
        CancellationToken ct)
    {
        var photographer = await _dbContext.Photographers.
            Include(x=>x.InstagramDetail)
            .Where(x=>x.AppUserId == authUserId)
            .SingleOrDefaultAsync(ct);

        if (photographer is null)
        {
            throw new InvalidOperationException($"Unable to find photographer with authUserId: {authUserId}");
        }
        
        if (photographer.InstagramDetail != null)
        {
            photographer.InstagramDetail.BusinessAccountId = instagramDetail.BusinessAccountId;
            photographer.InstagramDetail.LongLivedUserAccessToken = instagramDetail.LongLivedUserAccessToken;
            photographer.InstagramDetail.TokenCreatedAt =  instagramDetail.TokenCreatedAt;
            photographer.InstagramDetail.TokenExpiresOnUtc = instagramDetail.TokenExpiresOnUtc;
            photographer.InstagramDetail.TokenExpiresOnUtc = instagramDetail.TokenExpiresOnUtc;
            photographer.InstagramDetail.FollowersCount =  instagramDetail.FollowersCount;
            photographer.InstagramDetail.AverageMonthlyViews = instagramDetail.AverageMonthlyViews;
        }
        else
        {
            var photographerInstagramDetails = new PhotographerInstagramDetail
            {
                Id = 0,
                PhotographerId = photographer.Id,
                BusinessAccountId = instagramDetail.BusinessAccountId,
                LongLivedUserAccessToken = instagramDetail.LongLivedUserAccessToken,
                TokenExpiresOnUtc = instagramDetail.TokenExpiresOnUtc,
                TokenCreatedAt = instagramDetail.TokenCreatedAt,
                TokenLastRefreshedAt = instagramDetail.TokenLastRefreshedAt,
                FollowersCount = instagramDetail.FollowersCount,
                AverageMonthlyViews = instagramDetail.AverageMonthlyViews,
            };
            _dbContext.PhotographerInstagramDetails.Add(photographerInstagramDetails);
        }

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            
        }
    }
    
}