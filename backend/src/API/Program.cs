using Infrastructure;
using Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MigrateDatabase();
    await DataSeeder.SeedAsync(app.Services);
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok("OK"))
    .WithName("HealthCheck");

app.Run();
