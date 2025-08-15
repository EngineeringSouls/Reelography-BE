using Microsoft.AspNetCore.Http;
using Reelography.Shared.Enums;

namespace Reelography.Dto.Photographers.Registration.Request;

public class PhotographerAddPortfolioItemRequestDto
{
    public int Id { get; set; }
    public required MediaTypeEnum Type { get; set; }
    public MediaSourceEnum Source { get; set; }
    public string? MimeType { get; set; }
    public double? SizeInBytes { get; set; }
    public required int OccasionId { get; set; }
    public IFormFile? MediaFile { get; set; }
    // Instagram import path
    public string? RemoteUrl { get; set; }

    public string? FileName { get; set; }
    
    // Optional: persist IG media id if you have it later
    public string? InstagramMediaId { get; set; }}