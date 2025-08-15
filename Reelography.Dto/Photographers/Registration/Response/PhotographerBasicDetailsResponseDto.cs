using Reelography.Dto.Photographers.Registration.Request;

namespace Reelography.Dto.Photographers.Registration.Response;

public class PhotographerBasicDetailsResponseDto: PhotographerBasicDetailsRequestDto
{
    public string? FormattedAddress { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Postal { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public decimal? AverageRating { get; set; }
    public int? TotalReviewCount { get; set; }

    // Media
    public string? ProfileImageUrl { get; set; }
}