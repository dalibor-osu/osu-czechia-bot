using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCord.Hosting.Services;
using OsuCzechiaBot.Database;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Managers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Configure();

var app = builder.Build();
app.UseSerilogRequestLogging();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<OsuCzechiaBotDatabaseContext>();
    
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/authorize", async (
    [FromQuery(Name = "state")] ulong discordId,
    [FromQuery] string code,
    [FromServices] AuthManager authManager,
    CancellationToken cancellationToken) =>
{
    if (discordId < 1)
    {
        return Results.BadRequest();
    }

    return await authManager.AuthorizeUserAsync(discordId, code, cancellationToken);
});

app.AddModules(typeof(Program).Assembly);
await app.RunAsync();