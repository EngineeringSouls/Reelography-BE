using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reelography.Api.Contents;
using Reelography.Api.Helper;
using Reelography.Dto.Notification;
using Reelography.Notification.Interfaces;
using Reelography.Service.Contracts.User;

namespace Reelography.Api.Controllers;

/// <summary>
/// Notification Service API Controller
/// </summary>
/// <param name="userService"></param>
/// <param name="authHelper"></param>
/// <param name="notificationService"></param>
[Route("api/notification")]
[ApiController]
public class NotificationServiceController(
    IUserService userService,
    AuthHelper authHelper,
    INotificationService notificationService): ControllerBase
{
    
    /// <summary>
    /// Request OTP 
    /// </summary>
    /// <param name="otpRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("request-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> RequestOtp([FromBody] OtpRequestDto otpRequestDto,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationService.SendOtpNotificationAsync(otpRequestDto, cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result));
    }
    
    /// <summary>
    /// Validate OTP
    /// </summary>
    /// <param name="otpValidateRequestDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("validate-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> ValidateOtp([FromBody] OtpValidateRequestDto otpValidateRequestDto,
        CancellationToken cancellationToken = default)
    {
        var result = await notificationService.ValidateOtpRequestAsync(otpValidateRequestDto, cancellationToken);
        return Ok(ApiResponse.SuccessResponse(result));
    }
    
}