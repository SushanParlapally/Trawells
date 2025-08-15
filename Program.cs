using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TravelDesk.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TravelDesk.Interface;
using TravelDesk.Models;
using TravelDesk.Service;

var builder = WebApplication.CreateBuilder(args);

// Add Render.io compatibility
var port = Environment.GetEnvironmentVariable("PORT") ?? "7075";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// --- Unified Configuration Section ---
// Supabase Client Initialization
try
{
    var supabaseUrl = builder.Configuration["Supabase:Url"];
    var supabaseKey = builder.Configuration["Supabase:Key"];
    
    Console.WriteLine($"[DIAGNOSTIC] Supabase URL from config: '{supabaseUrl ?? "--- IS NULL ---"}'");
    Console.WriteLine($"[DIAGNOSTIC] Supabase AnonKey from config: '{(string.IsNullOrEmpty(supabaseKey) ? "--- IS NULL ---" : "SET")}'");
    
    if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
    {
        Console.WriteLine("Supabase configuration found. Storage features will be available.");
    }
    else
    {
        Console.WriteLine("Supabase configuration is MISSING. Storage features will be disabled.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Supabase initialization failed: {ex.Message}");
}

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.MaxDepth = 64;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development-if-not-set";
Console.WriteLine($"[DIAGNOSTIC] JWT Key from config: '{(string.IsNullOrEmpty(jwtKey) ? "--- IS NULL ---" : "SET")}'");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Simplified for robust deployment
            ValidateAudience = false, // Simplified for robust deployment
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configure Entity Framework (This part was already perfect)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"[DIAGNOSTIC] Connection String from config: '{(string.IsNullOrEmpty(connectionString) ? "--- IS NULL OR EMPTY ---" : "SET")}'");

builder.Services.AddDbContext<TravelDeskContext>(options =>
{
    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("[FATAL] Database connection string not found. Database services will fail.");
    }
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(120);
    });
    
    // Configure PostgreSQL to handle DateTime properly
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
});

// Add Application Services
builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings")); // This correctly binds the whole section
builder.Services.AddScoped<IEmailService, EmailService>();

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(
                "https://trawells.netlify.app",
                "https://travel-desk-app.netlify.app",
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// --- Database Initialization ---
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<TravelDeskContext>();
        Console.WriteLine("Attempting database connection...");
        
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful.");
            context.Database.EnsureCreated();
            Console.WriteLine("Database initialization complete.");
        }
        else
        {
            Console.WriteLine("[FATAL] Cannot connect to database.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[FATAL] Database initialization failed: {ex.Message}");
    }
}

// --- HTTP Request Pipeline ---
app.UseSwagger();
app.UseSwaggerUI();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => "TravelDesk API is running! Visit /swagger for API documentation.");
app.MapMethods("/health", new[] { "GET", "HEAD" }, () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();