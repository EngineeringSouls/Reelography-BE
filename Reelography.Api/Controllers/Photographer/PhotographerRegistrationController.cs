using Microsoft.AspNetCore.Mvc;
using Reelography.Dto.Photographers.Registration.Request;
using Reelography.Service.Contracts.Photographer;

namespace Reelography.Api.Controllers.Photographer;

[ApiController]
[Route("api/registration/photographer")]
public class PhotographerRegistrationController : ControllerBase
{
    private readonly IPhotographerRegistrationService _photographerRegistrationService;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="photographerRegistrationService"></param>
    public PhotographerRegistrationController(IPhotographerRegistrationService photographerRegistrationService)
    {
        _photographerRegistrationService = photographerRegistrationService;
    }

    [HttpGet("state")]
    public async Task<ActionResult<PhotographerRegistrationStateDto>> GetCurrentState(CancellationToken ct)
    {
        int? uid = 1; //HttpContext.GetAppUserId();
        if (uid is null) return Unauthorized();
        var state = await _photographerRegistrationService.GetCurrentStateAsync(uid.Value, ct);
        return Ok(state);
    }

    [HttpPost("basic")]
    [RequestSizeLimit(104_857_600)] // 100MB
    public async Task<IActionResult> UpsertBasic([FromForm] PhotographerBasicDetailsRequestDto requestDto, CancellationToken ct)
    {
        int? uid = 1; //HttpContext.GetAppUserId();
        if (uid is null) return Unauthorized();
        var result = await _photographerRegistrationService.UpsertBasicDetailsAsync(uid.Value, requestDto, ct);
        return Ok(result);
    }
    [HttpPost("packages")]
    public async Task<IActionResult> AddPackages([FromBody] PhotographerAddPackagesListRequestDto dto, CancellationToken ct)
    {
        int? uid = 1; //HttpContext.GetAppUserId();
        if (uid is null) return Unauthorized();
        var result = await _photographerRegistrationService.AddPackagesAsync(uid.Value, dto, ct);
        return Ok(result);
    }

    [HttpPost("portfolio")]
    [RequestSizeLimit(2_147_483_648)] // 2GB for video batches
    public async Task<IActionResult> AddPortfolio([FromForm] PhotographerAddPortfolioListItemsRequestDto dto, CancellationToken ct)
    {
        int? uid = 1; //HttpContext.GetAppUserId();
        if (uid is null) return Unauthorized();
        var result = await _photographerRegistrationService.AddPortfolioAsync(uid.Value, dto, ct);
        return Ok(result);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CancellationToken ct)
    {
        int? uid = 1; //HttpContext.GetAppUserId();
        if (uid is null) return Unauthorized();
        var result = await _photographerRegistrationService.CompleteAsync(uid.Value, ct);
        return Ok(result);
    }
    
}
