using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reelography.Api.Contents;
using Reelography.Api.Helper;
using Reelography.Dto;
using Reelography.Service.Contracts;
using Reelography.Service.Contracts.User;

namespace Reelography.Api.Controllers;


/// <summary>
/// 
/// </summary>
/// <param name="loggerFactory"></param>
/// <param name="userService"></param>
/// <param name="authHelper"></param>
[Route("api/auth")]
[ApiController]
public class AuthController(ILoggerFactory loggerFactory,
    IUserService userService,
    AuthHelper authHelper,
    IAuthService authService)
    :ControllerBase
{
    private readonly ILogger<AuthController> _logger = loggerFactory.CreateLogger<AuthController>();

    /// <summary>
    /// Generates the jwt token against the given user id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="deviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("generate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateToken([FromQuery] int id,
        [FromQuery] string deviceId, 
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserClaimDto(id, cancellationToken);
        var token = authHelper.GenerateJwtToken(user);
        var refreshToken = authHelper.GenerateRefreshToken();
        await authService.InsertRefreshTokenAsync(refreshToken, user.Id,deviceId);
        return Ok(ApiResponse.SuccessResponse(new  TokenDto{ Token = token, RefreshToken = refreshToken }));
    }
    
    [HttpGet("token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromQuery] int id,
        [FromQuery] string deviceId, 
        CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserClaimDto(id, cancellationToken);
        var token = authHelper.GenerateJwtToken(user);
        var refreshToken = authHelper.GenerateRefreshToken();
        await authService.InsertRefreshTokenAsync(refreshToken, user.Id,deviceId);
        return Ok(ApiResponse.SuccessResponse(new  TokenDto{ Token = token, RefreshToken = refreshToken }));
    }

}