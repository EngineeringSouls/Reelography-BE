using Microsoft.EntityFrameworkCore;
using Reelography.Api.Helper;
using Reelography.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
SqlHelper.ConnectionString = connectionString;

builder.Services.AddDbContext<ReelographyDbContext>(options =>
    options.UseSqlServer(SqlHelper.GetConnection(), sqlOptionsBuilder =>
    {
        sqlOptionsBuilder.EnableRetryOnFailure();
        sqlOptionsBuilder.MigrationsAssembly(typeof(ReelographyDbContext).Assembly.FullName);
    }), ServiceLifetime.Transient);var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.Run();