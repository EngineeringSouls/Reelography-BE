using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Reelography.Data;
using Reelography.Dto.Master;
using Reelography.Service.Contracts.Master;
using Reelography.Shared.Enums;

namespace Reelography.Service.Master;

public sealed class OccasionPackageMappingService : IOccasionPackageMappingService
{
    private readonly ReelographyDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="dbContext"></param>
    public OccasionPackageMappingService(ReelographyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<OccasionDto>> GetOccasionsAsync(CancellationToken ct)
    {
        return await _dbContext.OccasionTypes              // <-- adjust DbSet name here if needed
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new OccasionDto{Id = x.Id, Name = x.Name, Description = x.Description})
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PricingUnitDto>> GetPricingUnitsAsync(CancellationToken ct)
    {
        return await _dbContext.PricingUnitTypes               // <-- adjust DbSet name here if needed
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new PricingUnitDto{Id = x.Id, Name = x.Name, Description = x.Description})
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServicePackagesDto>> GetServicePackagesAsync(CancellationToken ct)
    {
        return await _dbContext.ServicePackageTypes            // <-- adjust DbSet name here if needed
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ServicePackagesDto{Id = x.Id, Name = x.Name, Description = x.Description })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<OccasionPackageMappingDto>> GetOccasionPackageMappingsAsync(CancellationToken ct)
    {
        return await _dbContext.OccasionPackageMappings    // <-- adjust DbSet name here if needed
            .AsNoTracking()
            .Select(x => new OccasionPackageMappingDto{
                Id = x.Id,
                OccasionTypeId = (OccasionEnum)x.OccasionId,
                ServicePackageId = (ServicePackageEnum)x.ServicePackageTypeId!,
                PricingUnitId = x.PricingUnitId
            })
            .ToListAsync(ct);
    }
}
