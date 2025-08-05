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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configure Entity Framework - Use SQLite for production, SQL Server for development
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? 
                      builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TravelDeskContext>(options =>
{
    // Use SQLite for production (Render deployment), SQL Server for development
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        // Use SQLite for production (Render deployment)
        options.UseSqlite("Data Source=traveldesk.db");
    }
});

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
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
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS before other middleware
app.UseCors();

// Ensure authentication is used
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
