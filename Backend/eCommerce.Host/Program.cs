using eCommerce.Application.DependencyInjection;
using eCommerce.Infrastructure.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------------
// Serilog configuration
// ---------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("Application is building...");

// ---------------------
// Dependency Injection
// ---------------------
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithOrigins("http://localhost:7137");
    });
});


try
{
    var app = builder.Build();

    app.UseInfrastructureService();      
    app.UseSerilogRequestLogging();      
    app.UseCors("AllowAll");


    app.UseHttpsRedirection();
    app.UseAuthentication();            
    app.UseAuthorization();

    // ---------------------
    // Swagger in dev
    // ---------------------
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();

    Log.Information("Application is running...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed!");
}
finally
{
    Log.CloseAndFlush();
}
