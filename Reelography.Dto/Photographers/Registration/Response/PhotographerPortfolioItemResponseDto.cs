using Reelography.Dto.Photographers.Registration.Request;

namespace Reelography.Dto.Photographers.Registration.Response;

public class PhotographerPortfolioItemResponseDto: PhotographerAddPortfolioItemRequestDto
{
    public int Id { get; set; }
    public string MediaUrl { get; set; } = "";
    public string? ThumbnailUrl { get; set; }
}