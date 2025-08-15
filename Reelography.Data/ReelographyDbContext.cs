using Microsoft.EntityFrameworkCore;
using Reelography.Entities;

namespace Reelography.Data;

/// <summary>
/// ReelographyDbContext
/// </summary>
public class ReelographyDbContext: DbContext
{
    /// <summary>
    /// ReelographyDbContext
    /// </summary>
    /// <param name="options"></param>
    public ReelographyDbContext(DbContextOptions<ReelographyDbContext> options) : base(options){}

    #region  master
    
    public DbSet<MediaSource>  MediaSources { get; set; }
    public DbSet<MediaType>  MediaTypes { get; set; }
    public DbSet<OccasionPackageMapping>  OccasionPackageMappings { get; set; }
    public DbSet<OccasionType>  OccasionTypes { get; set; }
    public DbSet<PricingUnitType>  PricingUnitTypes { get; set; }
    public DbSet<ServicePackageType>  ServicePackageTypes { get; set; }

    #endregion

    #region Photographer

    public DbSet<Photographer>  Photographers { get; set; }
    public DbSet<FavouritePhotographer>  FavouritePhotographers { get; set; }
    public DbSet<PhotographerGooglePlaceDetail>  PhotographerGooglePlaceDetails { get; set; }
    public DbSet<PhotographerGooglePlaceReview>  PhotographerGooglePlaceReviews { get; set; }
    public DbSet<PhotographerInAppReview>  PhotographerInAppReviews { get; set; }
    public DbSet<PhotographerInstagramDetail>  PhotographerInstagramDetails { get; set; }
    public DbSet<PhotographerPackageDetail>  PhotographerPackageDetails { get; set; }
    public DbSet<PhotographerPortfolio>  PhotographerPortfolios { get; set; }
    public DbSet<PhotographerType>  PhotographerTypes { get; set; }

    #endregion
    
    /// <summary>
    /// Override OnModelCreating to seed static data
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        #region  Master Mappings

        modelBuilder.Entity<ServicePackageType>()
            .HasMany(x=>x.OccasionPackageMappings)
            .WithOne(x=>x.ServicePackageType)
            .HasForeignKey(x=>x.ServicePackageTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<OccasionType>()
            .HasMany(x=>x.OccasionPackageMappings)
            .WithOne(x=>x.OccasionType)
            .HasForeignKey(x=>x.OccasionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PricingUnitType>()
            .HasMany(x=>x.OccasionPackageMappings)
            .WithOne(x=>x.PricingUnitType)
            .HasForeignKey(x=>x.PricingUnitId)
            .OnDelete(DeleteBehavior.Restrict);
        
        #endregion


        #region  SeedData

        SeedStaticData.SeedAppStaticData(modelBuilder);

        #endregion

    }
}