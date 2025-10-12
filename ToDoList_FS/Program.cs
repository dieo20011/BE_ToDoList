using MongoDB.Driver;
using System.Text;
using ToDoList_FS;
using ToDoList_FS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use the PORT environment variable (for Render)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "8080");
    serverOptions.ListenAnyIP(port);
    
    // Only configure HTTPS on development environment
    if (builder.Environment.IsDevelopment())
    {
        serverOptions.ListenAnyIP(7291, listenOptions =>
        {
            listenOptions.UseHttps();
        });
    }
});

// Configure HTTPS redirection to use a specific port in development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpsRedirection(options =>
    {
        options.HttpsPort = 7291;
    });
}

// Add services to the container.
builder.Services.AddControllers();

// Add health checks
builder.Services.AddHealthChecks();

// Get MongoDB connection string from environment or fallback to hardcoded value
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? 
    "mongodb+srv://duyentran2491991:iPQTfs3rbS3Q1CBk@todolist.ineop.mongodb.net/?retryWrites=true&w=majority";

// Remove any ssl/tlsAllowInvalidCertificates from connection string if present
mongoConnectionString = mongoConnectionString.Replace("&ssl=true", "").Replace("ssl=true", "").Replace("&tlsAllowInvalidCertificates=true", "").Replace("tlsAllowInvalidCertificates=true", "");

builder.Services.AddSingleton<IMongoClient>(s =>
{
    try
    {
        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));
        // Always enforce TLS 1.2
        settings.SslSettings = new SslSettings 
        { 
            EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 
        };
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
        return new MongoClient(settings);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"MongoDB connection error: {ex}");
        throw;
    }
});

builder.Services.AddScoped<MongoDBService>();

// Configure Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

// Allow CORS with a more flexible policy for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
       policy => policy
           .WithOrigins(
               "http://localhost:4200",
               "https://todolist-angular-tau.vercel.app",
               "https://todolist-angular-tau-vercel.app",
               "https://todolist-angular.vercel.app"
           )
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials()
           .WithExposedHeaders("Content-Disposition")
       );
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get JWT Secret from environment or fallback to hardcoded value
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "banhxeo0210_abc1234567890abcdef";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Use CORS first before any other middleware
app.UseCors("CorsPolicy");

// Add this line to serve static files (for Swagger UI)
app.UseStaticFiles();

// Always enable Swagger in all environments for this project
app.UseSwagger();
app.UseSwaggerUI();

// Add health check endpoint
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync($"{{\"status\": \"{report.Status}\", \"version\": \"1.0.0\"}}");
    }
});

// Simple ping endpoint for Render health checks
app.MapGet("/ping", () => "pong");

// In production, Render handles HTTPS, so we don't need to redirect
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
