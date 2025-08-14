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

// Initialize Supabase Client
try
{
    var supabaseUrl = builder.Configuration["Supabase:Url"]; // Not sensitive, can use config
    var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY") ?? builder.Configuration["Supabase:Key"];
    
    if (!string.IsNullOrEmpty(supabaseUrl) && !string.IsNullOrEmpty(supabaseKey))
    {
        Console.WriteLine("Supabase configuration found. Storage features will be available.");
    }
    else
    {
        Console.WriteLine("Supabase configuration is missing. Storage features will be disabled.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Supabase initialization failed: {ex.Message}");
}

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Ignore cycles to avoid adding $id and $ref metadata
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.MaxDepth = 64; // Increase depth if needed
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development-only";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TravelDesk"; // Not sensitive, can use config
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TravelDesk"; // Not sensitive, can use config

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configure Entity Framework - Use PostgreSQL for production, SQLite for development
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                      builder.Configuration.GetConnectionString("DefaultConnection");

// Fix connection string issues and convert pooler to direct connection
if (!string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine($"Original connection string: {connectionString}");
    
    // Convert pooler connection to direct connection for Entity Framework
    if (connectionString.Contains("pooler.supabase.com:6543"))
    {
        connectionString = connectionString.Replace("aws-0-ap-south-1.pooler.supabase.com:6543", "db.pkhlhfpknxjaqruarvhi.supabase.co:5432");
        Console.WriteLine("Converted pooler connection to direct connection");
    }
    
    // Ensure proper SSL mode for Supabase
    if (!connectionString.Contains("sslmode="))
    {
        connectionString += connectionString.Contains("?") ? "&sslmode=require" : "?sslmode=require";
        Console.WriteLine("Added sslmode=require for Supabase");
    }
    
    Console.WriteLine($"Final connection string: {connectionString}");
}

builder.Services.AddDbContext<TravelDeskContext>(options =>
{
    // Force PostgreSQL for Supabase testing
    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    Console.WriteLine($"Environment variable ASPNETCORE_ENVIRONMENT = {env}");
    
    // Always use PostgreSQL for now to test Supabase
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
        npgsqlOptions.CommandTimeout(120);
    });
    Console.WriteLine("Using PostgreSQL (Supabase) database connection");
});

// Add Supabase Storage Service
builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(
                "https://trawells.netlify.app",
                "https://travel-desk-app.netlify.app",
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // 24 hours
    });
});

// Configure Email Service
builder.Services.Configure<EmailSettings>(options =>
{
    var emailSection = builder.Configuration.GetSection("EmailSettings");
    options.SmtpServer = emailSection["SmtpServer"] ?? "smtp.gmail.com";
    options.SmtpPort = int.Parse(emailSection["SmtpPort"] ?? "587");
    options.SenderEmail = emailSection["SenderEmail"] ?? "noreply@traveldesk.com";
    options.SenderPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? emailSection["SenderPassword"] ?? string.Empty;
});
builder.Services.AddScoped<IEmailService, EmailService>(); // Register the EmailService with IEmailService

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<TravelDeskContext>();
        Console.WriteLine("Attempting database connection...");
        
        // Test connection first
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful");
            // Only ensure created if we can connect
            context.Database.EnsureCreated();
            Console.WriteLine("Database initialization complete");
        }
        else
        {
            Console.WriteLine("Cannot connect to database - skipping initialization");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
        Console.WriteLine("Application will continue without database initialization");
        // Don't crash the app - continue without database initialization
    }
}

// Configure the HTTP request pipeline.
// Enable Swagger in all environments for API documentation
app.UseSwagger();
app.UseSwaggerUI();

// Add a simple health check endpoint that supports both GET and HEAD
app.MapMethods("/", new[] { "GET", "HEAD" }, () => "TravelDesk API is running! Visit /swagger for API documentation.");
app.MapMethods("/health", new[] { "GET", "HEAD" }, () => new { status = "healthy", timestamp = DateTime.UtcNow });



// Use CORS before other middleware
app.UseCors();

// Ensure authentication is used
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
