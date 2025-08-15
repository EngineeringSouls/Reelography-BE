using Reelography.Shared.Records;

namespace Reelography.Service.External.Contracts;

public interface IGooglePlacesService
{
    Task<GooglePlaceDetails> GetPlaceDetailsAsync(string? placeId, CancellationToken ct);
}