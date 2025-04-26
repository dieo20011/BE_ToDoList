using MongoDB.Driver;
using System.Text;
using ToDoList_FS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// **Cấu hình Kestrel trước khi build**
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // Use port from environment variable (for Render) or fallback to defaults
    var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "5000");
    serverOptions.ListenAnyIP(port); // HTTP for Render
    
    // Only configure HTTPS on development environment
    if (builder.Environment.IsDevelopment())
    {
        serverOptions.ListenAnyIP(7291, listenOptions =>
        {
            listenOptions.UseHttps(); // HTTPS
        });
    }
});

// Add services to the container.
builder.Services.AddControllers();

// Get MongoDB connection string from environment or fallback to hardcoded value
var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? 
    "mongodb+srv://duyentran2491991:iPQTfs3rbS3Q1CBk@todolist.ineop.mongodb.net/?retryWrites=true&w=majority&ssl=true";

builder.Services.AddSingleton<IMongoClient>(s =>
{
    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConnectionString));
    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
    return new MongoClient(settings);
});

builder.Services.AddScoped<MongoDBService>();

// Allow CORS with a more flexible policy for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
       policy => policy
           .WithOrigins(
               "http://localhost:4200", 
               "https://todolist-frontend.onrender.com", // Add your frontend URL on Render
               "https://*.onrender.com" // Allow any Render subdomain
           )
           .AllowAnyHeader()
           .AllowAnyMethod()
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

// Always enable Swagger in all environments for this project
app.UseSwagger();
app.UseSwaggerUI();

// Use the updated CORS policy
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// In production, Render handles HTTPS, so we don't need to redirect
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapControllers();

// Only set explicit URLs in development
if (app.Environment.IsDevelopment())
{
    app.Urls.Add("http://localhost:5148");
    // app.Urls.Add("https://localhost:7291"); // Bỏ comment nếu cần HTTPS
}

app.Run();
