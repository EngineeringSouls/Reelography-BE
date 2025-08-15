using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Reelography.Shared.Enums;

namespace Reelography.Dto.Photographers.Registration.Request;

public class PhotographerBasicDetailsRequestDto
{
    [Required, MaxLength(120)] public required string Name { get; set; }
    [Required, MaxLength(120)] public required string StudioName { get; set; }
    [MaxLength(2000)] public required string AboutInfo { get; set; }
    public PhotographerTypeEnum PhotographerTypeId { get; set; }
    public required string? GooglePlaceId { get; set; }
    public required int YearsExperience { get; set; }
    
    public IFormFile? ProfileImage { get; set; }

}