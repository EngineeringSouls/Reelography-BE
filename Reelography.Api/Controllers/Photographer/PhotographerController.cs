using Microsoft.AspNetCore.Mvc;
using Reelography.Dto.Master;
using Reelography.Dto.Photographers.Registration.Response;
using Reelography.Service.Contracts.Photographer;

namespace Reelography.Api.Controllers.Photographer;

[ApiController]
[Route("api/photographer")]
public class PhotographerController : ControllerBase
{
    private readonly IPhotographerService _photographerService;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="photographerService"></param>
    public PhotographerController(IPhotographerService photographerService)
    {
        _photographerService = photographerService;
    }
    
    
    // NEW: Step data GETs
    [HttpGet("basic")]
    public async Task<ActionResult<PhotographerBasicDetailsResponseDto?>> GetBasic(CancellationToken ct)
    {
        var uid = 1;
        var dto = await _photographerService.GetBasicAsync(uid, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("packages")]
    public async Task<ActionResult<List<PhotographerPackageResponseDto>>> GetPackages(CancellationToken ct)
    {
        var uid = 1;
        var dto = await _photographerService.GetPackagesAsync(uid, ct);
        return Ok(dto);
    }

    [HttpGet("portfolio")]
    public async Task<ActionResult<List<PhotographerPortfolioItemResponseDto>>> GetPortfolio(CancellationToken ct)
    {
        var uid = 1;
        var dto = await _photographerService.GetPortfolioAsync(uid, ct);
        return Ok(dto);
    }

    [HttpGet("available-occasions")]
    public async Task<ActionResult<List<OccasionDto>>> GetPhotographerOccasions(CancellationToken ct)
    {
        var uid = 1;
        var dto = await _photographerService.GetPhotographerOccasionsAsync(uid, ct);
        return Ok(dto);
    }
}