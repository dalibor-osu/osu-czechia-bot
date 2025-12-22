using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using OsuCzechiaBot.Configuration;
using OsuCzechiaBot.Constants;
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
                Content = string.Format(BotMessages.Commands.Authorize.AlreadyAuthorized, authorizedUser.GetMarkdownOsuProfileLink()),
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        var existingUser = await userManager.GetGuildUserAsync(userId, true);
        if (existingUser is null)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = string.Format(BotMessages.Commands.Authorize.NotAMember, configurationAccessor.Discord.InviteLink),
                Flags = MessageFlags.Ephemeral
            }));
            return;
        }

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = string.Format(BotMessages.Commands.Authorize.AuthorizeWithLink, GetAuthUrl(userId)),
            Flags = MessageFlags.Ephemeral
        }));
    }

    [SlashCommand("unlink", "Unlinks your osu! profile from your Discord account")]
    public async Task UnlinkAsync()
    {
        ulong discordId = Context.User.Id;
        var existingUser = await userManager.GetGuildUserAsync(discordId, true);
        if (existingUser is null)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = BotMessages.Commands.Unlink.NotAMember,
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
            Content = BotMessages.Commands.Unlink.Unlinked,
            Flags = MessageFlags.Ephemeral
        });
    }

    private string GetAuthUrl(ulong userId)
    {
        return $"https://osu.ppy.sh/oauth/authorize?client_id={configurationAccessor.Osu.ClientId}&response_type=code&scope=identify&state={userId}";
    }
}