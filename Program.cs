using SpotifyRequestManagement.Controllers;
using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Forzar HTTP en puerto 5000
builder.WebHost.UseUrls("http://0.0.0.0:5000");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<SpotifyAuthService>();
builder.Services.AddSingleton<AuthToken>();
builder.Services.AddSingleton<SearchByQueryController>();
builder.Services.AddHttpClient<SpotifyApiRequest>();
builder.Services.AddHttpClient<LastFMApiRequest>();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
               "http://localhost:5173",
               "http://192.168.137.1:5173",
               "https://spotify-backend.agreeablemushroom-8c2dff51.westus2.azurecontainerapps.io",
               "https://musical-recommender-frontend.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();
app.MapHub<ProgressHub>("/progresshub");

app.Run();