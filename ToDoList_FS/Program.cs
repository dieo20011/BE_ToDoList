using MongoDB.Driver;
using System.Text;
using ToDoList_FS;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IMongoClient>(s =>
{
    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl("mongodb+srv://duyentran2491991:iPQTfs3rbS3Q1CBk@todolist.ineop.mongodb.net/?retryWrites=true&w=majority&ssl=true\r\n"));
    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
    return new MongoClient(settings);

});

builder.Services.AddScoped<MongoDBService>();

// Allow CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost4200",
       policy => policy
           .WithOrigins("http://localhost:4200")
           .AllowAnyHeader()
           .AllowAnyMethod());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **THÊM AUTHENTICATION TRƯỚC `builder.Build()`**
var key = Encoding.ASCII.GetBytes("banhxeo0210_abc1234567890abcdef");
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
        IssuerSigningKey = new SymmetricSecurityKey(key), // Sử dụng biến `key`
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost4200");

// **Thêm Authentication trước Authorization**
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();

