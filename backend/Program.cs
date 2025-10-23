using TentRentalSaaS.Api.Models;
using TentRentalSaaS.Api.Services;
using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer(options =>
{
    options.Authority = "https://accounts.google.com";
    options.Audience = Environment.GetEnvironmentVariable("SERVICE_URL");
    options.RequireHttpsMetadata = true;
});

builder.Services.AddAuthorization();

// Configure rate limiting to prevent abuse
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.RealIpHeader = "X-Real-IP";
    options.HttpStatusCode = 429;
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/bookings",
            Period = "5m",
            Limit = 3  // 3 booking attempts per 5 minutes per IP
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/bookings/quote",
            Period = "1m",
            Limit = 10  // 10 quote requests per minute
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Period = "1m",
            Limit = 5  // 5 login attempts per minute
        },
        new RateLimitRule
        {
            Endpoint = "GET:/api/auth/verify",
            Period = "1m",
            Limit = 10  // 10 token verifications per minute
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Period = "1s",
            Limit = 10  // 10 requests per second for all other endpoints
        }
    };
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            var allowedOriginsRaw = builder.Configuration["ALLOWED_ORIGINS"];
            
            if (string.IsNullOrWhiteSpace(allowedOriginsRaw))
            {
                // NEVER allow all origins in production - this is a security risk
                if (builder.Environment.IsProduction())
                {
                    throw new InvalidOperationException(
                        "ALLOWED_ORIGINS environment variable must be set in production!");
                }
                
                // Only in development, allow localhost
                policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            }
            else
            {
                var allowedOrigins = allowedOriginsRaw
                    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                      .WithHeaders("Content-Type", "Authorization")
                      .AllowCredentials();
            }
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

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (TentRentalSaaS.Api.Helpers.BookingConflictException ex)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply rate limiting before CORS and authentication
app.UseIpRateLimiting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
