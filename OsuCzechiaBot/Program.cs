using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Exceptions;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Managers;
using OsuCzechiaBot.Models;
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