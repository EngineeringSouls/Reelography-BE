using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reelography.Api.Contents;
using Reelography.Api.Helper;
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
public class AuthController(ILoggerFactory loggerFactory, IUserService userService, AuthHelper authHelper)
    :ControllerBase
{
    private readonly ILogger<AuthController> _logger = loggerFactory.CreateLogger<AuthController>();
    /// <summary>
    /// Generates the jwt token against the given user id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("generate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateToken([FromQuery] int userId)
    {
        var user = await userService.GetUserClaimDto(userId);
        var token = authHelper.GenerateJwtToken(user);
        return Ok(ApiResponse.SuccessResponse(token));
    }

}