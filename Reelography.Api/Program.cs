using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Reelography.Api.Helper;
using Reelography.Data;
using Reelography.Service.Contracts.Master;
using Reelography.Service.Contracts.Photographer;
using Reelography.Service.External;
using Reelography.Service.External.Contracts;
using Reelography.Service.Master;
using Reelography.Service.Photographer;
using Reelography.Shared.Options;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
SqlHelper.ConnectionString = connectionString;
builder.Services.AddDbContext<ReelographyDbContext>(options =>
    options.UseSqlServer(SqlHelper.GetConnection(), sql =>
    {
        sql.EnableRetryOnFailure();
        sql.MigrationsAssembly(typeof(ReelographyDbContext).Assembly.FullName);
    }), ServiceLifetime.Transient);

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reelography API", Version = "v1" });

    // Bearer auth
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// CORS (if calling directly from http://localhost:3000 without Next rewrites)
builder.Services.AddCors(o => o.AddPolicy("Dev", p => p
    .WithOrigins("http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

// External services
builder.Services.Configure<GooglePlacesOptions>(builder.Configuration.GetSection("GooglePlaces"));
builder.Services.AddHttpClient<IGooglePlacesService, GooglePlacesService>();

// Program.cs (before Build())
builder.Services.Configure<BunnyStorageOptions>(builder.Configuration.GetSection("BunnyStorage"));
builder.Services.Configure<BunnyStreamOptions>(builder.Configuration.GetSection("BunnyStream"));
builder.Services.AddHttpClient<IStorageService, BunnyHybridStorageService>();


builder.Services.Configure<InstagramOptions>(builder.Configuration.GetSection("Instagram"));
builder.Services.AddScoped<IInstagramService, InstagramService>();
// ---- Application services ----
builder.Services.AddScoped<IPhotographerRegistrationService, PhotographerRegistrationService>();
builder.Services.AddScoped<IOccasionPackageMappingService, OccasionPackageMappingService>();
builder.Services.AddScoped<IPhotographerService, PhotographerService>();

// ---- (Optional) allow large multipart uploads for portfolio/videos ----
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; // 2 GB
});

builder.Services.AddOptions<InstagramOptions>()
    .Bind(builder.Configuration.GetSection("Instagram"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientId), "Instagram ClientId missing")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ClientSecret), "Instagram ClientSecret missing")
    .Validate(o => !string.IsNullOrWhiteSpace(o.RedirectUri), "Instagram RedirectUri missing")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Scopes), "Instagram Scopes missing")
    .ValidateOnStart();

builder.Services.AddOptions<OAuthOptions>()
    .Bind(builder.Configuration.GetSection("OAuth"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.StateSecret), "StateSecret missing")
    .ValidateOnStart();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reelography API v1");
    c.RoutePrefix = "swagger"; // http://localhost:5186/swagger
});

// Pipeline
app.UseHttpsRedirection();
app.UseCors("Dev");
// app.UseMiddleware<JwtAuthMiddleware>(); // keep this if already added (it allowlists /swagger)
app.MapControllers();
app.Run();
