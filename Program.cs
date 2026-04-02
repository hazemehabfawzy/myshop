using System.Text;
using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TechVault.API.Data;
using TechVault.API.Helpers;
using TechVault.API.Services.Implementations;
using TechVault.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "TechVaultDefaultSecretKey_32CharsMin";

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Controllers & Validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                );
            return new BadRequestObjectResult(new { errors });
        };
    });

// Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TechVault API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// DI Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ITagService, TagService>();

// Helpers
builder.Services.AddScoped<JwtManager>();

var app = builder.Build();

// Database Migration & Seeding with Retry Logic for Docker
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<AppDbContext>();

    bool dbSuccess = false;
    int retryCount = 0;
    while (!dbSuccess && retryCount < 10)
    {
        try
        {
            await context.Database.MigrateAsync();
            await DbSeeder.SeedAsync(context);
            dbSuccess = true;
            logger.LogInformation("Database migrated and seeded successfully.");
        }
        catch (Exception ex)
        {
            retryCount++;
            logger.LogWarning($"Database attempt {retryCount} failed. Retrying in 10s... {ex.Message}");
            await Task.Delay(10000);
        }
    }
}

// Pipeline configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

// Recurring Jobs
RecurringJob.AddOrUpdate("CheckOverdueOrders", () => JobHelper.CheckOverdueOrders(), Cron.Daily);
RecurringJob.AddOrUpdate("LowStockReport", () => JobHelper.GenerateLowStockReport(), "0 0 * * 0"); // Weekly Sunday 00:00

app.Run();

// Static Job Helper for Hangfire
public static class JobHelper
{
    public static async Task CheckOverdueOrders()
    {
        // This is a placeholder for the actual logic which should ideally be in a service
        // But for recurrence, we need a static method or a way to resolve services
        Console.WriteLine($"{DateTime.UtcNow}: Checking overdue orders...");
    }

    public static async Task GenerateLowStockReport()
    {
        Console.WriteLine($"{DateTime.UtcNow}: Generating low stock report...");
    }
}
