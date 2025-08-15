
// Reelography.Service/External/BunnyHybridStorageService.cs
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Reelography.Service.External.Contracts;
using Reelography.Shared.Options;
using Reelography.Shared.Records;

namespace Reelography.Service.External.Contracts;

public sealed class BunnyHybridStorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly BunnyStorageOptions _bunnyStorageOptions;
    private readonly BunnyStreamOptions _bunnyStreamOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="bunnyStorageOptions"></param>
    /// <param name="bunnyStreamOptions"></param>
    public BunnyHybridStorageService(HttpClient httpClient,
        IOptions<BunnyStorageOptions> bunnyStorageOptions,
        IOptions<BunnyStreamOptions> bunnyStreamOptions)
    {
        _httpClient = httpClient;
        _bunnyStorageOptions = bunnyStorageOptions.Value;
        _bunnyStreamOptions = bunnyStreamOptions.Value;
    }

    public async Task<StoragePutResult> PutImageAsync(Stream stream, string fileName, string contentType, CancellationToken ct)
    {
        var safe = Sanitize(fileName);
        var path = $"{_bunnyStorageOptions.RootPath}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}-{safe}";
        var url = $"https://storage.bunnycdn.com/{_bunnyStorageOptions.StorageZoneName}/{path}";

        using var req = new HttpRequestMessage(HttpMethod.Put, url);
        req.Headers.Add("AccessKey", _bunnyStorageOptions.ApiKey);
        req.Content = new StreamContent(stream);
        req.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        var res = await _httpClient.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var cdnUrl = $"{_bunnyStorageOptions.CdnBase.TrimEnd('/')}/{path}";
        return new StoragePutResult("bunny-storage", path, cdnUrl, cdnUrl); // thumb=original (Optimizer can make variants)
    }

    public async Task<StoragePutResult> PutVideoAsync(Stream stream, string fileName, string contentType, CancellationToken ct)
    {
        // 1) Create video
        var createUrl = $"https://video.bunnycdn.com/library/{_bunnyStreamOptions.LibraryId}/videos";
        using var createReq = new HttpRequestMessage(HttpMethod.Post, createUrl);
        createReq.Headers.Add("AccessKey", _bunnyStreamOptions.ApiKey);
        createReq.Content = new StringContent(JsonSerializer.Serialize(new { title = fileName }),
            System.Text.Encoding.UTF8, "application/json");
        var createRes = await _httpClient.SendAsync(createReq, ct);
        createRes.EnsureSuccessStatusCode();
        var created = JsonSerializer.Deserialize<BunnyCreateVideoResponse>(
            await createRes.Content.ReadAsStringAsync(ct))!;

        // 2) Upload binary
        var uploadUrl = $"https://video.bunnycdn.com/library/{_bunnyStreamOptions.LibraryId}/videos/{created.guid}";
        using var uploadReq = new HttpRequestMessage(HttpMethod.Put, uploadUrl);
        uploadReq.Headers.Add("AccessKey", _bunnyStreamOptions.ApiKey);
        uploadReq.Content = new StreamContent(stream);
        uploadReq.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        var upRes = await _httpClient.SendAsync(uploadReq, ct);
        upRes.EnsureSuccessStatusCode();

        var cdn = _bunnyStreamOptions.CdnBase.Replace("{{LIB}}", _bunnyStreamOptions.LibraryId);
        var playbackUrl = $"{cdn}/{created.guid}/playlist.m3u8";
        var thumb = $"{cdn}/{created.guid}/thumbnail.jpg";
        return new StoragePutResult("bunny-stream", created.guid, playbackUrl, thumb);
    }
    
    
    public async Task<StoragePutResult> PutVideoUsingRemoteUrlAsync(string remoteUrl, CancellationToken ct)
    {
        // 1) Create video
        var createUrl = $"https://video.bunnycdn.com/library/{_bunnyStreamOptions.LibraryId}/videos/fetch";
        using var createReq = new HttpRequestMessage(HttpMethod.Post, createUrl);
        createReq.Headers.Add("AccessKey", _bunnyStreamOptions.ApiKey);
        createReq.Content = new StringContent(JsonSerializer.Serialize(new { url = remoteUrl }),
            System.Text.Encoding.UTF8, "application/json");
        var createRes = await _httpClient.SendAsync(createReq, ct);
        createRes.EnsureSuccessStatusCode();
        var response = await createRes.Content.ReadAsStringAsync(ct);
        var created = JsonSerializer.Deserialize<BunnyDumpVideoResponse>(response!);


        var cdn = _bunnyStreamOptions.CdnBase.Replace("{{LIB}}", _bunnyStreamOptions.LibraryId);
        var playbackUrl = $"{cdn}/{created!.Id}/playlist.m3u8";
        var thumb = $"{cdn}/{created.Id}/thumbnail.jpg";
        return new StoragePutResult("bunny-stream", created.Id, playbackUrl, thumb);
    }

    public async Task DeleteAsync(string assetId, CancellationToken ct)
    {
        // detect storage vs stream by prefix pattern (we store storage path with slashes, stream is a GUID)
        if (assetId.Contains('/'))
        {
            var url = $"https://storage.bunnycdn.com/{_bunnyStorageOptions.StorageZoneName}/{assetId}";
            using var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Add("AccessKey", _bunnyStorageOptions.ApiKey);
            var res = await _httpClient.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
        }
        else
        {
            var url = $"https://video.bunnycdn.com/library/{_bunnyStreamOptions.LibraryId}/videos/{assetId}";
            using var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Add("AccessKey", _bunnyStreamOptions.ApiKey);
            var res = await _httpClient.SendAsync(req, ct);
            res.EnsureSuccessStatusCode();
        }
    }

    private static string Sanitize(string name)
    {
        var n = string.Concat(name.Where(c => char.IsLetterOrDigit(c) || c is '.' or '-' or '_'));
        return string.IsNullOrWhiteSpace(n) ? "file" : n.ToLowerInvariant();
    }

    private sealed class BunnyCreateVideoResponse
    {
        public string guid { get; set; } = string.Empty;
        public long videoLibraryId { get; set; }
    }

    private sealed class BunnyDumpVideoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }
}
