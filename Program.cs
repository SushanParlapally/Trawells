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
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"]; // Not sensitive, can use config
var jwtAudience = builder.Configuration["Jwt:Audience"]; // Not sensitive, can use config

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
    Console.WriteLine($"Using PostgreSQL (Supabase) with connection: {connectionString}");
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
            .AllowCredentials();
    });
});

// Configure Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>(); // Register the EmailService with IEmailService

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TravelDeskContext>();
    context.Database.EnsureCreated();
    
    // Add sample data for SQLite (development)
    if (builder.Environment.IsDevelopment() && !context.Users.Any())
    {
        // Add IT department
        var itDept = new Department { DepartmentName = "IT", IsActive = true };
        context.Departments.Add(itDept);
        
        // Add Admin role
        var adminRole = new Role { RoleName = "Admin", IsActive = true };
        context.Roles.Add(adminRole);
        
        // Add admin user
        var adminUser = new User 
        { 
            FirstName = "Admin", 
            LastName = "User", 
            Email = "work.sushanparlapally@gmail.com", 
            Password = "sushan@123", 
            Role = adminRole,
            Department = itDept,
            IsActive = true
        };
        context.Users.Add(adminUser);
        
        context.SaveChanges();
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
