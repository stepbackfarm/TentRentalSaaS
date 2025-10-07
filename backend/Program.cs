using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Explicitly build configuration
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine($"Current Environment: {builder.Environment.EnvironmentName}");

// --- BEGIN CONFIGURATION DUMP ---
Console.WriteLine("--- Dumping all configuration key-values ---");
foreach (var config in builder.Configuration.AsEnumerable().OrderBy(c => c.Key))
{
    var key = config.Key;
    var value = config.Value;
    if (key.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0 ||
        key.IndexOf("key", StringComparison.OrdinalIgnoreCase) >= 0 ||
        key.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0)
    {
        value = "REDACTED";
    }
    Console.WriteLine($"{key} = {value}");
}
Console.WriteLine("--- End of configuration dump ---");
// --- END CONFIGURATION DUMP ---

// Add services to the container.

// Build connection string from environment variables for production
if (builder.Environment.IsProduction())
{
    var dbHost = builder.Configuration["DB_HOST"];
    var dbName = builder.Configuration["DB_NAME"];
    var dbUser = builder.Configuration["DB_USER"];
    var dbPassword = builder.Configuration["DB_PASSWORD"]; // Injected from Secret Manager by the deployment service

    var connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword}";

    builder.Services.AddDbContext<ApiDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Use the local connection string for development
    builder.Services.AddDbContext<ApiDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IGeocodingService, GoogleMapsGeocodingService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://tent-rental-hh1bx2kh8-davids-projects-15ffe845.vercel.app",
                                "http://localhost:5173",
                                "http://localhost:5174")
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure to use PORT environment variable for Cloud Run
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

var app = builder.Build();

// Automatically apply EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
