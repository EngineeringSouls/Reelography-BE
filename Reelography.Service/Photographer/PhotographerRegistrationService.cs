
// Reelography.Services/Registration/PhotographerRegistrationService.cs
using Microsoft.EntityFrameworkCore;
using Reelography.Data;
using Reelography.Dto.Photographers.Registration.Request;
using Reelography.Entities;
using Reelography.Service.Contracts.Photographer;
using Reelography.Service.External.Contracts;
using Reelography.Shared.Enums;

namespace Reelography.Service.Photographer;

public sealed class PhotographerRegistrationService : IPhotographerRegistrationService
{
    private readonly ReelographyDbContext _dbContext;
    private readonly IGooglePlacesService _googlePlacesService;
    private readonly IStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="googlePlacesService"></param>
    /// <param name="storageService"></param>
    /// <param name="httpClientFactory"></param>
    public PhotographerRegistrationService(ReelographyDbContext dbContext, 
        IGooglePlacesService googlePlacesService, 
        IStorageService storageService,
        IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext; 
        _googlePlacesService = googlePlacesService; 
        _storageService = storageService;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// GetCurrentStateAsync
    /// </summary>
    /// <param name="appUserId"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<PhotographerRegistrationStateDto> GetCurrentStateAsync(int appUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .AsNoTracking()
            .Include(x => x.PackageDetails)
            .Include(x => x.PortfolioDetails)
            .Include(x => x.InstagramDetail)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);

        if (p is null)
            return new PhotographerRegistrationStateDto
            {
                OnboardingStatus = PhotographerOnboardingStatusEnum.BasicDetails,
                HasPortfolio = false,
                HasPackages = false,
                HasConnectedWithInstagram = false,
            };

        return new PhotographerRegistrationStateDto
        {
            OnboardingStatus = (PhotographerOnboardingStatusEnum)p.OnboardingStepId!,
            HasPortfolio = p.PortfolioDetails?.Count > 0,
            HasPackages = p.PackageDetails?.Count > 0,
            HasConnectedWithInstagram = !string.IsNullOrWhiteSpace(p.InstagramDetail?.LongLivedUserAccessToken)
        };
    }
    
    
    /// <summary>
    /// Upserts basic details (first step)
    /// </summary>
    /// <param name="appUserId"></param>
    /// <param name="detailsRequestDto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<object> UpsertBasicDetailsAsync(int appUserId, PhotographerBasicDetailsRequestDto detailsRequestDto, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .Include(x => x.GooglePlaceDetail)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);

        if (p is null)
        {
            p = new Entities.Photographer()
            {
                Id = 0,
                AppUserId = appUserId,
                Name = detailsRequestDto.Name,
                StudioName = detailsRequestDto.StudioName,
                AboutInfo = detailsRequestDto.AboutInfo,
                PhotographerTypeId = (int)detailsRequestDto.PhotographerTypeId,
                YearsExperience = detailsRequestDto.YearsExperience,
                OnboardingStepId = (int)PhotographerOnboardingStatusEnum.PackagesDetails,
                IsFeatured = false,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = appUserId.ToString()
            };
            _dbContext.Photographers.Add(p);
        }
        else
        {
            p.Name = detailsRequestDto.Name;
            p.StudioName = detailsRequestDto.StudioName;
            p.AboutInfo = detailsRequestDto.AboutInfo;
            p.PhotographerTypeId = (int)detailsRequestDto.PhotographerTypeId;
            p.YearsExperience = detailsRequestDto.YearsExperience;
            p.ModifiedOn = DateTime.UtcNow;
            if (p.OnboardingStepId == (int)PhotographerOnboardingStatusEnum.BasicDetails)
                p.OnboardingStepId = (int)PhotographerOnboardingStatusEnum.PackagesDetails;
            _dbContext.Photographers.Update(p);
        }

        // Google Place details + reviews
        var place = await _googlePlacesService.GetPlaceDetailsAsync(detailsRequestDto.GooglePlaceId, ct);
        if (p.GooglePlaceDetail is null)
        {
            p.GooglePlaceDetail = new PhotographerGooglePlaceDetail
            {
                Id = 0,
                PhotographerId = p.Id,
                FormattedAddress = place.FormattedAddress!,
                GooglePlaceId = place.PlaceId!,
                Line1 = place.AddressLine1, Line2 = place.AddressLine2,
                City = place.City, State = place.State,
                Country = place.Country, Postal = place.Postal,
                Lat = (float?)place.Lat, Lng = (float?)place.Lng,
                AverageRating = (decimal)(place.RatingAvg ?? 0),
                TotalReviewCount = place.RatingCount ?? 0,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = appUserId.ToString(),
                PhotographerGoogleReviews = place.Reviews.Select(r => 
                    new PhotographerGooglePlaceReview()
                    {
                        Id = 0, 
                        PhotographerGooglePlaceDetailId = 0,
                        ReviewerName = r.AuthorName,
                        ReviewerProfilePicUrl = r.ProfilePhotoUrl, 
                        StarRating = (decimal)r.Rating,
                        ReviewComment = r.Text ?? string.Empty,
                        CreatedOn = r.CreateTimeUtc,
                        CreatedBy = appUserId.ToString()
                    }).ToList()
            };
        }
        else
        {
            var existingReviews = await _dbContext.PhotographerGooglePlaceReviews
                .Where(r => r.PhotographerGooglePlaceDetailId == p.GooglePlaceDetailId)
                .ToListAsync(ct);
            _dbContext.PhotographerGooglePlaceReviews.RemoveRange(existingReviews);
            
            p.GooglePlaceDetail.FormattedAddress = place.FormattedAddress!;
            p.GooglePlaceDetail.GooglePlaceId = place.PlaceId!;
            p.GooglePlaceDetail.Line1 = place.AddressLine1; p.GooglePlaceDetail.Line2 = place.AddressLine2;
            p.GooglePlaceDetail.City = place.City; p.GooglePlaceDetail.State = place.State;
            p.GooglePlaceDetail.Country = place.Country; p.GooglePlaceDetail.Postal = place.Postal;
            p.GooglePlaceDetail.Lat = (float?)place.Lat; p.GooglePlaceDetail.Lng = (float?)place.Lng;
            p.GooglePlaceDetail.AverageRating = (decimal)(place.RatingAvg ?? 0);
            p.GooglePlaceDetail.TotalReviewCount = place.RatingCount ?? 0;
            p.GooglePlaceDetail.PhotographerGoogleReviews = place.Reviews.Select(r =>
                new PhotographerGooglePlaceReview()
                {
                    Id = 0,
                    PhotographerGooglePlaceDetailId = p.GooglePlaceDetail.Id,
                    ReviewerName = r.AuthorName,
                    ReviewerProfilePicUrl = r.ProfilePhotoUrl,
                    StarRating = (decimal)r.Rating,
                    ReviewComment = r.Text ?? string.Empty,
                    CreatedOn = r.CreateTimeUtc,
                    CreatedBy = appUserId.ToString()
                }).ToList();
        }

        // Optional profile image upload
        if (detailsRequestDto.ProfileImage is not null && detailsRequestDto.ProfileImage.Length > 0)
        {
            var existingProfilePhotoStorageId = p.ProfileImageStorageId;
            if (existingProfilePhotoStorageId is not null)
            {
                await _storageService.DeleteAsync(existingProfilePhotoStorageId, ct);
            }
            
            await using var s = detailsRequestDto.ProfileImage.OpenReadStream();
            var put = await _storageService.PutImageAsync(s, detailsRequestDto.ProfileImage.FileName, detailsRequestDto.ProfileImage.ContentType, ct);
            p.ProfileImageUrl = put.Url;
            p.ProfileImageStorageId = put.AssetId; // ensure these two columns exist on Photographer
        }

        try
        {
            await _dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            
        }

        return new { p.Id, OnboardingStatus = p.OnboardingStepId };
    }

    public async Task<object> AddPackagesAsync(int appUserId, PhotographerAddPackagesListRequestDto dto, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .Include(x => x.PackageDetails)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);
        if (p is null) throw new InvalidOperationException("Complete basic details first.");

        if (p.PackageDetails?.Count > 0)
        {
            _dbContext.RemoveRange(p.PackageDetails);
        }
        
        foreach (var entity in dto.Packages.Select(pkg => new PhotographerPackageDetail 
                 {
                     Id = 0,
                     IsActive = true,
                     PhotographerId = p.Id,
                     Title = pkg.Title,
                     OccasionPackageMappingId = pkg.OccasionPackageMappingId,
                     MinPrice = pkg.BasePriceInr,
                     Description = pkg.Description,
                     CreatedOn = DateTime.UtcNow,
                     CreatedBy = appUserId.ToString()
                 }))
        {
            _dbContext.PhotographerPackageDetails.Add(entity);
        }

        // Advance to Portfolio
        p.OnboardingStepId = (int)PhotographerOnboardingStatusEnum.PortfolioDetails;

        await _dbContext.SaveChangesAsync(ct);
        return new { p.Id, OnboardingStatus = p.OnboardingStepId  };
    }

    public async Task<object> AddPortfolioAsync(int appUserId, PhotographerAddPortfolioListItemsRequestDto dto, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .Include(photographer => photographer.PortfolioDetails)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);
        if (p is null) throw new InvalidOperationException("Complete basic details first.");

        var idsInRequest = dto.PortfolioItems.Select(p => p.Id).ToList();
        
        
        var existingPortfoliosToUpdate = p.PortfolioDetails!
            .Where((x => idsInRequest.Contains(x.Id)))
            .ToList();
        
        var existingPortfoliosToDelete = p.PortfolioDetails!
            .Where(x=>!idsInRequest.Contains(x.Id))
            .ToList();

        var newPortfolioItemsToAdd = dto.PortfolioItems
            .Where(x => !existingPortfoliosToUpdate.Select(x => x.Id).Contains(x.Id))
            .ToList();
        
        
        if (existingPortfoliosToDelete.Count != 0)
        {
            foreach (var portfolioItem in existingPortfoliosToDelete)
            {
                await _storageService.DeleteAsync(portfolioItem.StorageAssetId, ct);
            }
            _dbContext.PhotographerPortfolios.RemoveRange(existingPortfoliosToDelete);
        }
        if (existingPortfoliosToUpdate.Count != 0)
        {
            foreach (var portfolioItem in existingPortfoliosToUpdate)
            {
                portfolioItem.OccasionId = dto.PortfolioItems
                    .Single(x => x.Id == portfolioItem.Id).OccasionId;
                portfolioItem.ModifiedOn = DateTime.UtcNow;
            }
            _dbContext.PhotographerPortfolios.UpdateRange(existingPortfoliosToUpdate);
        }
        
        foreach (var item in newPortfolioItemsToAdd)
        {
            string url;
            string? thumb = null;
            string? assetId;
            var isVideo = item.Type is MediaTypeEnum.Video;
            if (item.MediaFile is not null && item.MediaFile.Length > 0)
            {
                await using var s = item.MediaFile.OpenReadStream();
                var put = isVideo
                    ? await _storageService.PutVideoAsync(s, item.MediaFile.FileName, item.MediaFile.ContentType, ct)
                    : await _storageService.PutImageAsync(s, item.MediaFile.FileName, item.MediaFile.ContentType, ct);

                url = put.Url; thumb = put.ThumbUrl; assetId = put.AssetId;
            }
            else if (item.RemoteUrl is not null && item.RemoteUrl.Length > 0)
            {
                if (isVideo)
                {
                    var put = await _storageService.PutVideoUsingRemoteUrlAsync(item.RemoteUrl, ct);
                    url = put.Url; thumb = put.ThumbUrl; assetId = put.AssetId;

                }
                else
                {
                    using var hc = _httpClientFactory.CreateClient();
                    await using var s = await hc.GetStreamAsync(item.RemoteUrl, ct);
                    var put = await _storageService.PutImageAsync(s, "Test","image/jpeg", ct);
                    url = put.Url; thumb = put.ThumbUrl; assetId = put.AssetId;

                }
            }
            else
            {
                continue;
            }

            _dbContext.PhotographerPortfolios.Add(new PhotographerPortfolio
            {
                Id = 0,
                PhotographerId = p.Id,
                OccasionId = item.OccasionId,
                MediaUrl = url,
                ThumbnailUrl = thumb,
                IsApproved = false,
                MimeType = item.MimeType,
                SizeBytes = item.SizeInBytes,
                MediaTypeId = (int)item.Type,
                MediaSourceId = (int)item.Source,
                StorageAssetId = assetId,
                FileName = item.FileName,
                CreatedOn = DateTime.UtcNow,
                InstagramMediaId = item.InstagramMediaId,
                CreatedBy = p.Name
            });
        }

        if (p.OnboardingStepId == (int)PhotographerOnboardingStatusEnum.PortfolioDetails)
            p.OnboardingStepId = (int)PhotographerOnboardingStatusEnum.Draft;
        
        await _dbContext.SaveChangesAsync(ct);
        return new { p.Id, OnboardingStatus = p.OnboardingStepId  };
    }

    public async Task<object> CompleteAsync(int appUserId, CancellationToken ct)
    {
        var p = await _dbContext.Photographers
            .Include(x => x.PortfolioDetails)
            .Include(x => x.PackageDetails)
            .FirstOrDefaultAsync(x => x.AppUserId == appUserId, ct);

        if (p is null) throw new InvalidOperationException("Complete basic details first.");
        if (p.PortfolioDetails?.Count <= 0) throw new InvalidOperationException("Add at least one portfolio item.");
        if (p.PackageDetails?.Count <= 0) throw new InvalidOperationException("Add at least one package.");

        p.OnboardingStepId = (int)PhotographerOnboardingStatusEnum.Draft;
        await _dbContext.SaveChangesAsync(ct);
        return new { p.Id, OnboardingStatus = p.OnboardingStepId };
    }
}
