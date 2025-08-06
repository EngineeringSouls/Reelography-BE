using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Reelography.Data;

/// <summary>
/// Db Context Factory 
/// </summary>
public class ReelographyDbContextFactory : IDesignTimeDbContextFactory<ReelographyDbContext>
{
    /// <summary>
    /// Create the Db Context
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public ReelographyDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory());
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var builder = new DbContextOptionsBuilder<ReelographyDbContext>();
        var connectionString = configuration.GetConnectionString("DbConnection");
        builder.UseSqlServer(connectionString);
        return new ReelographyDbContext(builder.Options);
    }
}