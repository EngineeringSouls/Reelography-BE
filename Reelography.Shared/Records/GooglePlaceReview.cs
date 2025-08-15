namespace Reelography.Shared.Records;

public sealed record GooglePlaceReview
(
    string AuthorName,
    string? ProfilePhotoUrl,
    double Rating, 
    DateTime CreateTimeUtc,
    string? Text
);