using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using OsuCzechiaBot.Constants;
using OsuCzechiaBot.Database.DatabaseServices;
using OsuCzechiaBot.Managers;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Modules;

public class AdminModule(ReactionRoleManager reactionRoleManager, ApplicationSettingDatabaseService applicationSettingDatabaseService)
    : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("addreactrole", "Add new react role", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task AddReactRole(
        [SlashCommandParameter(Name = "emoji", Description = "Emoji to use for reaction")]
        string reactEmoji,
        [SlashCommandParameter(Name = "role", Description = "Role to use for reaction")]
        Role role,
        [SlashCommandParameter(Name = "description", Description = "Description of the role")]
        string? description = null)
    {
        bool success = await reactionRoleManager.AddAsync(reactEmoji, role, description);
        if (success)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = $"Successfully assigned reaction role {role} to emoji {reactEmoji}"
            }));
        }
        else
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = "An error occurred when adding reaction role."
            }));
        }
    }

    [SlashCommand("removereactrole", "Remove react role", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task RemoveReactRole(
        [SlashCommandParameter(Name = "role", Description = "Role to remove")]
        Role role,
        [SlashCommandParameter(Name = "removefromusers", Description = "Whether to remove the role from all users")]
        bool removeFromUsers)
    {
        bool success = await reactionRoleManager.RemoveAsync(role.Id, removeFromUsers);
        if (!success)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = $"An error occurred when removing reaction role. Is it really a reaction role?"
            }));
            return;
        }

        if (removeFromUsers)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = $"Role {role} was successfully removed from reaction roles and will be removed from all users"
            }));
        }
        else
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = $"Role {role} was successfully removed from reaction roles"
            }));
        }
    }

    [SlashCommand("forcereactionrolemessagerefresh", "Force refresh of reaction role message",
        DefaultGuildPermissions = Permissions.Administrator)]
    public async Task ForceReactRoleMessageRefresh()
    {
        await reactionRoleManager.UpdateRoleMessageAsync();
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = $"Reaction role message was refreshed : {reactionRoleManager.GetReactionRoleMessageLink()}"
        }));
    }

    [SlashCommand("changereactionrolemessage", "Change the reaction role message")]
    public async Task ChangeReactionRoleMessage(
        [SlashCommandParameter(Name = "message", Description = "New message to use as reaction role message", MaxLength = 1024)]
        string message,
        [SlashCommandParameter(Name = "refresh", Description = "Whether to refresh the reaction role message right away")]
        bool refresh = false)
    {
        await applicationSettingDatabaseService.AddOrUpdateAsync(new ApplicationSetting
        {
            Id = SettingsKeys.ReactionRoleMessageKey,
            Value = message
        });

        if (refresh)
        {
            await reactionRoleManager.UpdateRoleMessageAsync();
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
            {
                Content = $"Reaction role message was updated and refreshed: {reactionRoleManager.GetReactionRoleMessageLink()}"
            }));
            return;
        }

        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = $"Reaction role message was updated in database. To refresh it, please use the /forcereactionrolemessagerefresh command. Content will look like this:\n\n{await reactionRoleManager.GetRoleMessageContent()}"
        }));
    }
}