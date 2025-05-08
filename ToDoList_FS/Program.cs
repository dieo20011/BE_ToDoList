using MongoDB.Driver;
using System.Text;
using ToDoList_FS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

// Add services to the container.
builder.Services.AddControllers();

// Get MongoDB connection string from environment or fallback to hardcoded value
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? 
    "mongodb+srv://duyentran2491991:iPQTfs3rbS3Q1CBk@todolist.ineop.mongodb.net/?retryWrites=true&w=majority";

builder.Services.AddSingleton<IMongoClient>(s =>
{
    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));
    
    // Configure SSL/TLS settings correctly
    settings.SslSettings = new SslSettings 
    { 
        EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 
    };
    
    // Set server connection timeout
    settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    
    return new MongoClient(settings);
});

builder.Services.AddScoped<MongoDBService>();

// Allow CORS with a more flexible policy for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
       policy => policy
           .WithOrigins(
               "https://todolist-angular-tau.vercel.app",
               "https://todolist-angular-tau-vercel.app",
               "https://todolist-angular.vercel.app",
               "http://localhost:4200"
           )
           .AllowAnyHeader()
           .AllowAnyMethod()
           .WithExposedHeaders("Content-Disposition")
           .AllowCredentials());
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

// Always enable Swagger in all environments for this project
app.UseSwagger();
app.UseSwaggerUI();

// In production, Render handles HTTPS, so we don't need to redirect
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Only set explicit URLs in development
if (app.Environment.IsDevelopment())
{
    app.Urls.Add("http://localhost:5148");
    // app.Urls.Add("https://localhost:7291"); // Bỏ comment nếu cần HTTPS
}

app.Run();
