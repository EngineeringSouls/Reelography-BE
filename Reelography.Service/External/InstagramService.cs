
// Reelography.Services/Instagram/InstagramService.cs
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Reelography.Data;
using Reelography.Entities;
using Reelography.Service.External.Contracts;
using Reelography.Service.Helper;
using Reelography.Shared.Options;
using Reelography.Shared.Records;

namespace Reelography.Service.External;

public sealed class InstagramService : IInstagramService
{
    private readonly InstagramOptions _instagramOptions;
    private readonly OAuthOptions _authOptions;
    private readonly IHttpClientFactory _httpClient;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="instagramOptions"></param>
    /// <param name="authOptions"></param>
    /// <param name="httpClient"></param>
    public InstagramService(IOptions<InstagramOptions> instagramOptions,
        IOptions<OAuthOptions> authOptions,
        IHttpClientFactory httpClient)
    {
        _httpClient = httpClient;
        _instagramOptions = instagramOptions.Value;
        _authOptions = authOptions.Value;
    }

    public string CreateState(int authUserId)
    {
        var state = StateToken.Create(_authOptions.StateSecret, userId: authUserId, TimeSpan.FromMinutes(20));
        return state;
    }
    
    public (bool validateSuccess, int authUserId)  ValidateState(string state)
    {
        var (ok, userId) = StateToken.Validate(_authOptions.StateSecret, state);
        return (ok, userId);
    }
    
    public string BuildAuthorizationUrl(string state)
    {
        var clientId = _instagramOptions.ClientId;
        var redirect = Uri.EscapeDataString(_instagramOptions.RedirectUri);
        var scope = Uri.EscapeDataString(_instagramOptions.Scopes);

        // Instagram API with Instagram Login → Instagram domain (not Facebook)
        return $"https://api.instagram.com/oauth/authorize" +
               $"?client_id={clientId}" +
               $"&redirect_uri={redirect}" +
               $"&response_type=code" +
               $"&scope={scope}" +
               $"&state={state}";
    }

    public async Task<(string? longToken, double? expiresInSec)> ExchangeCodeAsync(string code, CancellationToken ct)
    {
        var httpClient = _httpClient.CreateClient();
        httpClient.BaseAddress = new Uri("https://api.instagram.com/");
        var form = new FormUrlEncodedContent(new Dictionary<string,string> {
            ["client_id"] = _instagramOptions.ClientId,
            ["client_secret"] = _instagramOptions.ClientSecret,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = _instagramOptions.RedirectUri,
            ["code"] = code
        });
        var res = await httpClient.PostAsync("oauth/access_token", form, ct);
        res.EnsureSuccessStatusCode();
        using var json = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
        var shortLivedIgToken = json.RootElement.GetProperty("access_token").GetString()!;
        Console.WriteLine("Short Loved token: "+shortLivedIgToken);
        var longLivedIgToken = await ExchangeLongLivedAsync(httpClient, shortLivedIgToken, ct);
        return longLivedIgToken;
    }
    
    public async Task<InstagramUserProfileWithMedia> GetUserProfileWithMediaAsync(string accessToken, CancellationToken ct)
    {
        var httpClient = _httpClient.CreateClient();
        httpClient.BaseAddress = new Uri("https://graph.instagram.com/");
        // Get user info including followers_count
        var userInfo = await httpClient.GetFromJsonAsync<JsonElement>(
            $"me?fields=id,username,account_type,media_count,followers_count&access_token={accessToken}", ct);

        var igUserId = userInfo.GetProperty("id").GetString()!;
        var username = userInfo.GetProperty("username").GetString()!;
        var followersCount = userInfo.GetProperty("followers_count").GetDouble();

        // Get latest 30 media
        var mediaResponse = await httpClient.GetFromJsonAsync<JsonElement>(
            $"me/media?fields=id,caption,media_type,media_url,thumbnail_url,permalink,timestamp&limit=30&access_token={accessToken}", ct);

        var mediaList = new List<InstagramMediaItem>();
        if (mediaResponse.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
        {
            mediaList.AddRange(data.EnumerateArray()
                .Select(media => new InstagramMediaItem(
                    Id: media.GetProperty("id").GetString()!, 
                    MediaType: media.GetProperty("media_type").GetString()!, 
                    MediaUrl: media.GetProperty("media_url").GetString()!, 
                    ThumbUrl: media.TryGetProperty("thumbnail_url", out var thumb) ? thumb.GetString() : null, 
                    Timestamp: DateTimeOffset.Parse(media.GetProperty("timestamp").GetString()!), 
                    Caption: media.TryGetProperty("caption", out var cap) ? cap.GetString() : null,
                    SizeInBytes: media.TryGetProperty("file_size", out var size) ? size.GetDouble() : null)));
        }

        var monthlyAvgViews = 40000; // await GetMonthlyAverageViewsAsync(httpClient, accessToken, ct);
        return new InstagramUserProfileWithMedia(igUserId, username, followersCount, mediaList, monthlyAvgViews);
    }
    
    private async Task<(string?, double?)> ExchangeLongLivedAsync(HttpClient httpClient, string shortLivedToken, CancellationToken ct)
    {
        const string baseUrl = "https://graph.instagram.com/access_token";
        var url = $"{baseUrl}?grant_type=ig_exchange_token" +
                  $"&client_secret={_instagramOptions.ClientSecret}" +
                  $"&access_token={shortLivedToken}";
        string? longLivedToken = null;
        double? expiresInSeconds = null;
        var response = await httpClient.GetAsync(url, ct);
        if (response!.IsSuccessStatusCode)
        {
            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            longLivedToken = json.RootElement.GetProperty("access_token").GetString()!;
            expiresInSeconds = json.RootElement.GetProperty("expires_in").GetDouble();
            await response.Content.ReadAsStringAsync(ct);
        }
        else
        {
            var error = await response!.Content.ReadAsStringAsync(ct);
            Console.WriteLine("Error: " + error);
        }
        return (longLivedToken,  expiresInSeconds);
    }


    // Get monthly average views using Instagram Insights
    public async Task<double> GetMonthlyAverageViewsAsync(HttpClient httpClient, string accessToken, CancellationToken ct)
    {
        httpClient.BaseAddress ??= new Uri("https://graph.instagram.com/");
        
        var since = DateTimeOffset.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
        var until = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd");

        // Get user insights for impressions/reach
        var insights = await httpClient.GetFromJsonAsync<JsonElement>(
            $"me/insights?metric=impressions,reach&period=day&since={since}&until={until}&access_token={accessToken}", ct);

        if (insights.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array)
        {
            var totalViews = 0.0;
            var dayCount = 0;

            foreach (var metric in data.EnumerateArray())
            {
                if (metric.TryGetProperty("values", out var values) && values.ValueKind == JsonValueKind.Array)
                {
                    foreach (var value in values.EnumerateArray())
                    {
                        if (value.TryGetProperty("value", out var val))
                        {
                            totalViews += val.GetDouble();
                            dayCount++;
                        }
                    }
                }
            }

            return dayCount > 0 ? totalViews / dayCount : 0;
        }

        return 0;
    }
    
    
    
    
    
    
    
 
}
