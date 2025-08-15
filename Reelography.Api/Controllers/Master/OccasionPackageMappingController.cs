using Microsoft.AspNetCore.Mvc;
using Reelography.Service.Contracts.Master;

namespace Reelography.Api.Controllers.Master;

[ApiController]
[Route("api/master")]
public class OccasionPackageMappingController: ControllerBase
{
    private readonly IOccasionPackageMappingService _occasionPackageMappingService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="occasionPackageMappingService"></param>
    public OccasionPackageMappingController(IOccasionPackageMappingService occasionPackageMappingService)
    {
        _occasionPackageMappingService = occasionPackageMappingService;
    }

    [HttpGet("occasions")]
    public async Task<IActionResult> Occasions(CancellationToken ct)
    {
        return Ok(await _occasionPackageMappingService.GetOccasionsAsync(ct));
    }

    [HttpGet("pricing-units")]
    public async Task<IActionResult> Units(CancellationToken ct)
    {
        return Ok(await _occasionPackageMappingService.GetPricingUnitsAsync(ct));
    }

    [HttpGet("service-packages")]
    public async Task<IActionResult> Packages(CancellationToken ct)
    {
        return Ok(await _occasionPackageMappingService.GetServicePackagesAsync(ct));
    }

    [HttpGet("occasion-mappings")]
    public async Task<IActionResult> Mappings(CancellationToken ct)
    {
        return Ok(await _occasionPackageMappingService.GetOccasionPackageMappingsAsync(ct));
    }
}