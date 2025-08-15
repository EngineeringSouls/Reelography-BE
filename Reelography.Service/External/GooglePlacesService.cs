using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Reelography.Service.External.Contracts;
using Reelography.Shared.Options;
using Reelography.Shared.Records;

namespace Reelography.Service.External;

/// <summary>
/// GooglePlacesService
/// </summary>
public class GooglePlacesService : IGooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly GooglePlacesOptions _googlePlacesOptions;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="options"></param>
    public GooglePlacesService(HttpClient httpClient, IOptions<GooglePlacesOptions> options)
    {
        _httpClient = httpClient;
        _googlePlacesOptions = options.Value;
    }

    public async Task<GooglePlaceDetails> GetPlaceDetailsAsync(string? placeId, CancellationToken ct)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/details/json?fields=name%2Cformatted_address%2Caddress_component%2Cgeometry%2Crating%2Cuser_ratings_total%2Creviews%2Ctypes&place_id={placeId}&key={_googlePlacesOptions.ApiKey}";
        var res = await _httpClient.GetFromJsonAsync<PlaceDetailsApiResponse>(url, ct) ?? throw new InvalidOperationException("No response");
        if (!string.Equals(res.status, "OK", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Places error: {res.status}");

        var r = res.result;
        var comps = r?.address_components ?? [];
        string? Get(string type) => comps.FirstOrDefault(c => c.types.Contains(type))?.long_name;

        var reviews = r?.reviews ?? [];

        var googlePlaceReviews = reviews
            .Select(review => new GooglePlaceReview(review.author_name, 
                review.profile_photo_url, 
                review.rating ?? 0, 
                DateTimeOffset.FromUnixTimeSeconds(review.time ?? 0).UtcDateTime, 
                review.text))
            .ToList();

        return new GooglePlaceDetails(
            placeId,
            r!.name,
            r!.name + ", "+ r!.formatted_address,
            Get("premise") ?? Get("street_number") ?? r.name,
            Get("route"),
            Get("locality") ?? Get("sublocality"),
            Get("administrative_area_level_1"),
            Get("country"),
            Get("postal_code"),
            r.geometry?.location?.lat,
            r.geometry?.location?.lng,
            r.rating,
            r.user_ratings_total,
            googlePlaceReviews
        );
    }
    
    private sealed class PlaceDetailsApiResponse
    {
        public string? status { get; set; }
        public PlaceResult? result { get; set; }
    }

    private sealed class PlaceResult
    {
        public string place_id { get; set; } = "";
        public string? name { get; set; }
        
        public string? formatted_address { get; set; }
        public double? rating { get; set; }
        public int? user_ratings_total { get; set; }
        public AddressComponent[]? address_components { get; set; }
        public Geometry? geometry { get; set; }
        public Review[]? reviews { get; set; }
    }

    private sealed class Review
    {
        public string? author_name { get; set; }
        public string? author_url { get; set; }
        public string? profile_photo_url { get; set; }
        public double? rating { get; set; }
        public string? text { get; set; }
        public long? time { get; set; }
    }

    private sealed class AddressComponent
    {
        public string long_name { get; set; } = "";
        public string[] types { get; set; } = Array.Empty<string>();
    }

    private sealed class Geometry
    {
        public Location? location { get; set; }
    }

    private sealed class Location
    {
        public double? lat { get; set; }
        public double? lng { get; set; }
    }
}