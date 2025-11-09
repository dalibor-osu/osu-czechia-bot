using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Modules;

public class AuthModule(ConfigurationAccessor configurationAccessor, UserManager userManager)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("authorize", "Authorize via osu! web")]
    public async Task Authorize()
    {
        ulong userId = Context.User.Id;
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = $"Please authorize via this link: {GetAuthUrl(userId)}",
            Flags = MessageFlags.Ephemeral
        }));
    }

    [SlashCommand("unlink", "Unlinks your osu! profile from your Discord account")]
    public async Task UnlinkAsync()
    {
        ulong discordId = Context.User.Id;
        bool success = await userManager.UnlinkUserAsync(discordId, Context.Interaction, CancellationToken.None);
        if (!success)
        {
            return;
        }

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Your osu! profile has been unlinked. You will have to authorize again using the */authorize* command.",
            Flags = MessageFlags.Ephemeral
        }));
    }

    private string GetAuthUrl(ulong userId)
    {
        return $"https://osu.ppy.sh/oauth/authorize?client_id={configurationAccessor.Osu.ClientId}&response_type=code&scope=identify&state={userId}";
    }
}