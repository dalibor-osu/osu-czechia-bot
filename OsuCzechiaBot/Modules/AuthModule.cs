using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Extensions;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Modules;

public class AuthModule(ConfigurationAccessor configurationAccessor, UserManager userManager)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("authorize", "Authorize via osu! web")]
    public async Task Authorize()
    {
        ulong userId = Context.User.Id;
        var authorizedUser = await userManager.GetAsync(userId);

        if (authorizedUser is { OsuId: > 0 })
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content =
                    $"You are already authorized with this {authorizedUser.GetMarkdownOsuProfileLink()}. If you'd like to link a different account, use the */unlink* command first.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        var existingUser = await userManager.GetGuildUserAsync(userId);
        if (existingUser is null)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content =
                    $"It looks like you are not member of osu! Czechia server and thus can't perform an authorization. Please join using this invite link first: {configurationAccessor.Discord.InviteLink}",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

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
        var existingUser = await userManager.GetGuildUserAsync(discordId);
        if (existingUser is null)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content =
                    $"It looks like you are not member of osu! Czechia server and thus can't perform an unlink. If you were verified and left the server, your profiles were automatically unlinked.",
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        bool success = await userManager.UnlinkUserAsync(discordId, Context.Interaction, CancellationToken.None);
        if (!success)
        {
            return;
        }

        await Context.Interaction.SendFollowupMessageAsync(new InteractionMessageProperties
        {
            Content = "Your osu! profile has been unlinked. You will have to authorize again using the */authorize* command.",
            Flags = MessageFlags.Ephemeral
        });
    }

    private string GetAuthUrl(ulong userId)
    {
        return $"https://osu.ppy.sh/oauth/authorize?client_id={configurationAccessor.Osu.ClientId}&response_type=code&scope=identify&state={userId}";
    }
}