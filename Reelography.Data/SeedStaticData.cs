using Microsoft.EntityFrameworkCore;
using Reelography.Entities;
using Reelography.Shared.Enums;
using Reelography.Shared.Extensions;

namespace Reelography.Data;

public class SeedStaticData
{
    private static readonly DateTime Date = DateTime.MinValue;
    public static void SeedAppStaticData(ModelBuilder modelBuilder)
    {
        SeedOccasionTypes(modelBuilder);
        SeedPricingUnitTypes(modelBuilder);
        SeedPhotographerTypes(modelBuilder);
        SeedMediaTypes(modelBuilder);
        SeedMediaSources(modelBuilder);
        SeedServicePackageTypes(modelBuilder);
        SeedOccasionPackageMappings(modelBuilder);
        SeedPhotographerOnboardingSteps(modelBuilder);
    }
    
    private static void SeedOccasionTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OccasionType>().HasData(
            Enum.GetValues<OccasionEnum>()
                .Select(e => new OccasionType()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true,
                    IsPremium = e is OccasionEnum.Wedding or OccasionEnum.PreWedding
                }).ToArray()
        );
    }
    private static void SeedPricingUnitTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PricingUnitType>().HasData(
            Enum.GetValues<PricingUnitEnum>()
                .Select(e => new PricingUnitType()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
    private static void SeedPhotographerTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhotographerType>().HasData(
            Enum.GetValues<PhotographerTypeEnum>()
                .Select(e => new PhotographerType()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
    private static void SeedMediaTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaType>().HasData(
            Enum.GetValues<MediaTypeEnum>()
                .Select(e => new MediaType()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
    private static void SeedMediaSources(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MediaSource>().HasData(
            Enum.GetValues<MediaSourceEnum>()
                .Select(e => new MediaSource()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
    
    private static void SeedServicePackageTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServicePackageType>().HasData(
            Enum.GetValues<ServicePackageEnum>()
                .Select(e => new ServicePackageType()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
    
    /// <summary>
    /// Seeding data for wedding for default packages (silver, gold etc..)
    /// </summary>
    /// <param name="modelBuilder"></param>
    private static void SeedOccasionPackageMappings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OccasionPackageMapping>().HasData(
            Enum.GetValues<ServicePackageEnum>()
                .Select(e => new OccasionPackageMapping()
                {
                    Id = (int)e,
                    OccasionId = (int)OccasionEnum.Wedding,
                    ServicePackageTypeId = (int)e,
                    PricingUnitId = (int)PricingUnitEnum.PerPackage,
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                }).ToArray()
        );
    }
    
    
    /// <summary>
    /// Seeding data for wedding for default packages (silver, gold etc..)
    /// </summary>
    /// <param name="modelBuilder"></param>
    private static void SeedPhotographerOnboardingSteps(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PhotographerOnboardingStep>().HasData(
            Enum.GetValues<PhotographerOnboardingStatusEnum>()
                .Select(e => new PhotographerOnboardingStep()
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = e.GetDescription(),
                    CreatedBy = "System-SeedData",
                    CreatedOn = Date,
                    IsActive = true
                }).ToArray()
        );
    }
}