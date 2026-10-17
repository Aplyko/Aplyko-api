using System.Text.Json.Serialization;
using AplykoApi;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("connPostgres")
                       ?? $"Host=localhost;Port=5432;Database={Environment.GetEnvironmentVariable("PG_DB_NAME")};Username={Environment.GetEnvironmentVariable("PG_USER")};Password={Environment.GetEnvironmentVariable("PG_PASSWORD_DEV")}";

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hot reloading working");

app.Run();
