using SpotifyRequestManagement.Models;
using SpotifyRequestManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<SpotifyAuthService>();
builder.Services.AddSingleton<AuthToken>();
builder.Services.AddHttpClient<SpotifyApiRequest>();
builder.Services.AddHttpClient<LastFMApiRequest>();
builder.Services.AddSingleton<MusicalGenerativeField>();
builder.Services.AddSingleton<SamplerContextualFilter>();
builder.Services.AddSingleton<MapSimplifiedTracks>();
builder.Services.AddSingleton<FilterAlgorithmHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
