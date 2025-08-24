using Microsoft.EntityFrameworkCore;
using Reelography.Entities;
using Reelography.Entities.User;
using Reelography.Shared.Enums;
using Reelography.Shared.Enums.User;
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
        
        SeedUserData(modelBuilder);
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



    #region User

    private static void SeedUserData(ModelBuilder modelBuilder)
    {
        SeedAuthUser(modelBuilder);
        SeedAuthUserRoles(modelBuilder);
    }

    private static void SeedAuthUser(ModelBuilder modelBuilder)
    {
        var adminUser = new AuthUser()
        {
            Id = 1,
            Email = "admin@admin.com",
            UserName = "admin",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            IsActive = true,
            FirstName = "Admin",
            LastName = "Name",
            EmailConfirmed = true,
        };
        
        var photographerUser = new AuthUser()
        {
            Id = 2,
            Email = "Photographer@Studio.com",
            UserName = "Photographer",
            NormalizedEmail = "Photographer@Studio.COM",
            NormalizedUserName = "Photographer@Studio.COM",
            IsActive = true,
            FirstName = "Photographer",
            LastName = "Name",
            EmailConfirmed = true,
        };
        
        var user = new AuthUser()
        {
            Id = 3,
            Email = "User@gmail.com",
            UserName = "User",
            NormalizedEmail = "User@Gmail.com",
            NormalizedUserName = "User@gmail.com",
            IsActive = true,
            FirstName = "User",
            LastName = "Name",
            EmailConfirmed = true,
        };
        modelBuilder.Entity<AuthUser>().HasData(adminUser, photographerUser, user);
    }
    private static void SeedAuthUserRoles(ModelBuilder modelBuilder)
    {
        int id = 0;
        modelBuilder.Entity<AuthUserRole>().HasData(
            new AuthUserRole()
            {
                Id = ++id,
                RoleName = AuthUserRoleEnum.Admin.GetDescription(),
                AuthUserId = 1,
                Role = AuthUserRoleEnum.Admin,
            },
            new AuthUserRole()
            {
                Id = ++id,
                RoleName = AuthUserRoleEnum.Photographer.GetDescription(),
                AuthUserId = 2,
                Role = AuthUserRoleEnum.Photographer,
            },
        new AuthUserRole()
            {
                Id = ++id,
                RoleName = AuthUserRoleEnum.User.GetDescription(),
                AuthUserId = 3,
                Role = AuthUserRoleEnum.User,
            }
            );
    }
    #endregion
}