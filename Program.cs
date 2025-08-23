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
using FluentValidation;
using TravelDesk.Hubs;
using Microsoft.AspNetCore.Http;
using TravelDesk.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Render.io compatibility
var port = Environment.GetEnvironmentVariable("PORT") ?? "7075";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// Supabase Client Initialization
var supabaseUrl = builder.Configuration["Supabase:Url"];
var supabaseKey = builder.Configuration["Supabase:Key"];
if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    Console.WriteLine("Supabase configuration is MISSING. Storage features will be disabled.");
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
});

// Add Security Services
builder.Services.AddScoped<TravelDesk.Services.IPasswordService, TravelDesk.Services.PasswordService>();

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<TravelDesk.Validators.UserCreateDtoValidator>();

// Add Application Services
builder.Services.AddScoped<ISupabaseStorageService, SupabaseStorageService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

// Add SignalR
builder.Services.AddSignalR();

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
        if (context.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful.");
            context.Database.EnsureCreated();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.MapMethods("/", new[] { "GET", "HEAD" }, () => "TravelDesk API is running! Visit /swagger for API documentation.");
app.MapMethods("/health", new[] { "GET", "HEAD" }, () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.MapFallbackToFile("index.html");

app.Run();

// --- THIS IS THE CRITICAL FIX FOR INTEGRATION TESTING ---
// By explicitly defining a public partial class Program at the END of the file,
// we make the application's entry point visible to the external TravelDesk.Tests project
// while still respecting the "top-level statements" C# language rule.
public partial class Program { }
// --- END OF FIX ---