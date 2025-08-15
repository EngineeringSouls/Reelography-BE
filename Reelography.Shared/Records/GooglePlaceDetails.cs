namespace Reelography.Shared.Records;

public sealed record GooglePlaceDetails(
        string? PlaceId,
        string? Name,
        string? FormattedAddress,
        string? AddressLine1,
        string? AddressLine2,
        string? City,
        string? State,
        string? Country,
        string? Postal,
        double? Lat,
        double? Lng,
        double? RatingAvg,
        int? RatingCount,
        List<GooglePlaceReview> Reviews
        );