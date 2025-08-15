// Reelography.Api/Controllers/Registration/PhotographerInstagramController.cs

using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reelography.Dto.Photographers.Registration.Request;
using Reelography.Entities;
using Reelography.Service.Contracts.Photographer;
using Reelography.Service.External;
using Reelography.Service.External.Contracts;
using Reelography.Shared.Enums;

namespace Reelography.Api.Controllers.Photographer;

[ApiController]
[Route("api/registration/photographer/instagram")]
public sealed class PhotographerInstagramController : ControllerBase
{
    private readonly IInstagramService _ig;
    private readonly IHttpClientFactory _httpClientFactory;
    IPhotographerService _photographerService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="ig"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="photographerService"></param>
    public PhotographerInstagramController(IInstagramService ig, 
        IHttpClientFactory httpClientFactory,
        IPhotographerService photographerService)
    {
        _ig = ig;
        _httpClientFactory = httpClientFactory;
        _photographerService = photographerService;
    }

    // Start: returns the Instagram Login URL and writes a short-lived anti-CSRF state cookie
    [HttpGet("start")]
    [AllowAnonymous]
    public IActionResult Start()
    {
        var authUserId = 1;
        var state = _ig.CreateState(authUserId);
        var url = _ig.BuildAuthorizationUrl(state);
        return Ok(new { authorizationUrl = url });
    }

    // OAuth callback: verify state, exchange code, resolve IG user id, stash tokens & close the popup
    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            return BadRequest("Missing code/state");

        var (validateSuccess, authUserId) = _ig.ValidateState(state);
        if (!validateSuccess) return BadRequest("Invalid state");

        Console.WriteLine("Code: "+code +", authUserId: "+authUserId);
        
        try
        {
            // 1) Exchange code -> long-lived token
            var (longToken, expiresInSec) = await _ig.ExchangeCodeAsync(code, ct);

            if (longToken == null)
            {
                return Content(
                    CallbackHtml(
                        JsonSerializer.Serialize(new { success = false, error = "Failed to get access token" })),
                    "text/html");
            }
            // 2) Get basic profile info with followers and average views
            var profile = await _ig.GetUserProfileWithMediaAsync(longToken, ct);

            var instagramDetail = new PhotographerInstagramDetailDTO
            {
                BusinessAccountId = profile.Username,
                LongLivedUserAccessToken = longToken,
                TokenExpiresOnUtc = DateTime.UtcNow.AddSeconds(expiresInSec.GetValueOrDefault()),
                TokenCreatedAt = DateTime.UtcNow,
                TokenLastRefreshedAt = DateTime.UtcNow,
                FollowersCount = profile.FollowersCount,
                AverageMonthlyViews = profile.MonthlyAverageViews
            };

            // 3) Save token + info to DB for authUserId
            await _photographerService.AddPhotographerInstagramBasicDetailsAsync(authUserId, instagramDetail, ct);
            
            // 4) Return success flag only
            return Content(CallbackHtml(JsonSerializer.Serialize(new { success = true })), "text/html");
        }
        catch (Exception ex)
        {
            return Content(CallbackHtml(JsonSerializer.Serialize(new { 
                success = false, 
                error = $"Failed to fetch Instagram data: {ex.Message}" 
            })), "text/html");
        }
    }
    
    // New API: Fetch Instagram media after connection is established
    [HttpGet("media")]
    public async Task<IActionResult> GetInstagramMedia(CancellationToken ct)
    {
        int authUserId = 1; // get from auth context or claims
        // 1) retrieve saved token for authUserId from DB
        var instDetails = await _photographerService.GetInstagramDetailAsync(authUserId, ct);
        if (instDetails == null || string.IsNullOrWhiteSpace(instDetails.LongLivedUserAccessToken))
            return Unauthorized("Instagram not connected");

        // 2) fetch latest media using saved token
        var userProfileWithMedia = await _ig.GetUserProfileWithMediaAsync(instDetails.LongLivedUserAccessToken, ct);

        // 3) return media only
        var mediaResult = userProfileWithMedia.MediaItems.Select(m => new
        {
            InstagramMediaId = m.Id,
            mediaType = m.MediaType.Equals("video", StringComparison.CurrentCultureIgnoreCase) ? MediaTypeEnum.Video : MediaTypeEnum.Image,
            source = MediaSourceEnum.Instagram,
            url = m.MediaUrl,
            thumbUrl = m.ThumbUrl,
            caption = m.Caption,
            timestamp = m.Timestamp,
            m.SizeInBytes
        });

        return Ok(mediaResult);
    }
    
    private static string CallbackHtml(string json) => $@"<!doctype html>
        <html><body>
        <script>
          (function() {{
            try {{
              if (window.opener && typeof window.opener.postMessage === 'function') {{
                window.opener.postMessage({json}, '*');
              }}
            }} catch (e) {{}}
            window.close();
          }})();
        </script>
        OK
        </body></html>";
}
