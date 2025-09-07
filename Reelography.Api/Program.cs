using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reelography.Api.Configuration;
using Reelography.Api.Helper;
using Reelography.Data;
using Reelography.Dto;

var builder = WebApplication.CreateBuilder(args);


#region Enable Swagger

if (builder.Configuration.GetValue<bool>("App:EnableSwagger"))
{
    builder.Services.AddOpenApi();
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);

        // ✅ Add JWT Bearer Auth to Swagger
        var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.",
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                securityScheme,
                Array.Empty<string>()
            }
        });
    });
}

#endregion

// Add Controller and Endpoint Support
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            ),
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var claims = ctx.Principal!;
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var deviceId = claims.FindFirst("device_id")?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(deviceId))
                {
                    ctx.Fail("Invalid token: missing device.");
                    return;
                }

                // OPTIONAL: validate against your DeviceSession table
                var db = ctx.HttpContext.RequestServices.GetRequiredService<ReelographyDbContext>();
                var valid = await db.DeviceSessions
                    .AnyAsync(s => s.AuthUserId.ToString() == userId && s.DeviceId == deviceId && !s.Revoked);
                if (!valid)
                {
                    ctx.Fail("Invalid or revoked session.");
                }
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    // Requires “role” claim with value “Admin”
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Requires “role” claim with value “User”
    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("User"));
    
    options.AddPolicy("PhotographerOnly", policy =>
        policy.RequireRole("Photographer"));
});

// ====================
// 🔧 JSON Options (System.Text.Json)
// ====================
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.AllowTrailingCommas = true;
    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
});


// ====================
// 📦 Swagger Setup (Conditionally enabled)
// ====================
if (builder.Configuration.GetValue<bool>("App:EnableSwagger"))
{
    builder.Services.AddOpenApi(); // your extension method
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
    });
}
// ====================
// CORS Policy Update
// ====================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ====================
// 🗄️ SQL Database Setup
// ====================
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
SqlHelper.ConnectionString = connectionString;

builder.Services.AddDbContext<ReelographyDbContext>(options =>
    options.UseSqlServer(SqlHelper.GetConnection(), sqlOptionsBuilder =>
    {
        sqlOptionsBuilder.EnableRetryOnFailure();
        sqlOptionsBuilder.MigrationsAssembly(typeof(ReelographyDbContext).Assembly.FullName);
        //sqlOptionsBuilder.UseNetTopologySuite();
    }));

builder.RegisterServices(); 

var app = builder.Build();

app.UseCors();
// ====================
// 📡 Routing
// ====================
app.UseRouting();

// ====================
// 🌍 Environment-specific Settings
// ====================
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// ====================
// 📊 Swagger Middleware & Root Redirect
// ====================
if (builder.Configuration.GetValue<bool>("App:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }
        await next();
    });
}


app.UseAuthentication(); // ✅ This is mandatory before UseAuthorization

// ✅ Built-in ASP.NET Core middleware required for `[Authorize]` to work
app.UseAuthorization(); 
// ====================
// 🧱 Other Middleware
// ====================
app.UseHttpsRedirection();

// ====================
// ✅ Endpoint Mapping (must come last)
// ====================
app.MapControllers();
app.Run();