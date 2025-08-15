using Reelography.Shared.Records;

namespace Reelography.Service.External.Contracts;

public interface IStorageService
{
    Task<StoragePutResult> PutImageAsync(Stream stream, string fileName, string contentType, CancellationToken ct);
    Task<StoragePutResult> PutVideoAsync(Stream stream, string fileName, string contentType, CancellationToken ct);
    Task DeleteAsync(string assetId, CancellationToken ct);
    Task<StoragePutResult> PutVideoUsingRemoteUrlAsync(string remoteUrl, CancellationToken ct);
}