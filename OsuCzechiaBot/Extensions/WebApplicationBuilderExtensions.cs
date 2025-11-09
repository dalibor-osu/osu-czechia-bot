using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using OsuCzechiaBot.Clients;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Exceptions;
using OsuCzechiaBot.Managers;
using Serilog;
using Serilog.Events;

namespace OsuCzechiaBot.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        var configurationAccessor = new ConfigurationAccessor(builder.Configuration);
        builder.Services.AddSingleton(configurationAccessor);

        builder.ConfigureDiscord();
        builder.ConfigureDatabase();

        builder.Services.AddMvc();
        builder.Services.AddSerilog(config =>
        {
            config
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                #if !DEBUG
                .WriteTo.File(path: "/app/logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7, restrictedToMinimumLevel: LogEventLevel.Warning)
                #endif
                .Enrich.FromLogContext();
        });

        builder.Services.AddSingleton(new RestClient(new BotToken(configurationAccessor.Discord.Token)));
        builder.Services.AddHttpClient<OsuHttpClient>(options =>
        {
            options.BaseAddress = new Uri("https://osu.ppy.sh");
            options.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
        });

        builder.Services.AddScoped<UserManager>();
        builder.Services.AddScoped<AuthManager>();

        return builder;
    }

    private static WebApplicationBuilder ConfigureDiscord(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddDiscordGateway(options => { options.Intents = GatewayIntents.All; })
            .AddApplicationCommands()
            .AddDiscordRest()
            .AddGatewayHandlers(typeof(Program).Assembly);
        return builder;
    }

    private static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        string databaseConnectionString = configuration.GetConnectionString("PostgreSQL") ??
                                          throw new ConfigurationException(
                                              "PostgreSQL connection string not found");

        builder.Services.AddDbContext<OsuCzechiaBotDatabaseContext>(options =>
        {
            options.UseNpgsql(databaseConnectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly("OsuCzechiaBot");
                optionsBuilder.MigrationsHistoryTable("__MigrationsHistory", OsuCzechiaBotDatabaseContext.SchemaName);
            });
            options.UseNpgsql(x => x.MigrationsAssembly("OsuCzechiaBot"));
        });

        builder.Services.AddScoped<AuthorizedUserDatabaseService>();

        return builder;
    }
}