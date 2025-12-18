using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCord.Hosting.Services;
using OsuCzechiaBot.Constants;
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
    HttpContext httpContext,
    [FromQuery(Name = "state")] ulong discordId,
    [FromQuery] string code,
    [FromServices] AuthManager authManager,
    [FromServices] UserManager userManager) =>
{
    if (discordId < 1)
    {
        return Results.BadRequest();
    }

    httpContext.Response.ContentType = MediaTypes.Html;
    await httpContext.Response.WriteAsync(HtmlResponses.AuthBeingProcessed);
    await httpContext.Response.Body.FlushAsync();

    try
    {
        string responseMessage = await authManager.AuthorizeUserAsync(discordId, code);
        await httpContext.Response.WriteAsync(responseMessage);
    }
    catch (Exception e)
    {
        Log.Error(e, "Something went wrong when authorizing user: {Message}", e.Message);
        await httpContext.Response.WriteAsync(HtmlResponses.AuthFailed);
    }
    
    return Results.Empty;
});

app.AddModules(typeof(Program).Assembly);
await app.RunAsync();