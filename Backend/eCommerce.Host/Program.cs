using eCommerce.Application.Configuration;
using eCommerce.Application.DependencyInjection;
using eCommerce.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration (Production only)
//if (builder.Environment.IsProduction())
//{
//    var keyVaultUrl = "https://ecommerce-zaighum-kv.vault.azure.net/";
//    builder.Configuration.AddAzureKeyVault(
//        new Uri(keyVaultUrl),
//        new DefaultAzureCredential()
//    );
//}

// ---------------------
// Serilog Configuration
// ---------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Application starting...");
Log.Information("Environment: {Environment}", builder.Environment.EnvironmentName);

// ---------------------
// Stripe Configuration
// ---------------------
builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));

var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrEmpty(stripeSecretKey))
{
    Stripe.StripeConfiguration.ApiKey = stripeSecretKey;
    Log.Information("✓ Stripe configured successfully");
}
else
{
    Log.Warning("⚠ Stripe:SecretKey not found - Stripe functionality will be disabled");
}

// ---------------------
// Service Registrations
// ---------------------
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

// ---------------------
// Controllers Configuration
// ---------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure routing
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

builder.Services.AddEndpointsApiExplorer();

// ---------------------
// Swagger Configuration
// ---------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "eCommerce API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ---------------------
// CORS Configuration
// ---------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
    
    // Keep AllowAll for development
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ---------------------
// Application Pipeline
// ---------------------
try
{
    var app = builder.Build();

    Log.Information("Building application pipeline...");

    // Ensure database is migrated and seed Roles
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            // Apply any pending EF Core migrations (creates missing tables like RefreshToken)
            var db = scope.ServiceProvider.GetRequiredService<eCommerce.Infrastructure.Data.AppDbContext>();
            await db.Database.MigrateAsync();

            try
            {
                // Enable WAL mode and set a busy timeout to reduce "database is locked" errors when using SQLite
                db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
                db.Database.ExecuteSqlRaw("PRAGMA busy_timeout=30000;");
                Log.Information("✓ SQLite PRAGMA applied: WAL + busy_timeout");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not apply SQLite PRAGMA settings");
            }

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "User", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    Log.Information("Creating role: {Role}", role);
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            Log.Information("✓ Database migrated and roles seeded successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error applying migrations or seeding roles");
        }
    }

    // Middleware Pipeline
    // Register custom infrastructure middleware (ExceptionMiddleware)
    app.UseInfrastructureService();

    app.UseSerilogRequestLogging();
    app.UseCors("AllowFrontend");

    if (app.Environment.IsDevelopment())
    {
        Log.Information("✓ Swagger UI enabled (Development mode)");
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "eCommerce API v1");
            options.RoutePrefix = "swagger"; // Swagger at: /swagger
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Map controllers
    app.MapControllers();

    Log.Information("Application pipeline built successfully");
    Log.Information("====================================");
    Log.Information("✓ Application ready!");
    Log.Information("✓ Swagger UI: https://localhost:7138/swagger (HTTPS) or http://localhost:5217/swagger (HTTP)");
    Log.Information("====================================");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}
