using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using OsuCzechiaBot.Managers;

namespace OsuCzechiaBot.Modules;

public class AdminModule(ReactionRoleManager reactionRoleManager) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("addreactrole", "Add new react role", DefaultGuildPermissions = Permissions.Administrator)]
    public async Task AddReactRole(
        [SlashCommandParameter(Name = "emoji", Description = "Emoji to use for reaction")]
        string reactEmoji,
        [SlashCommandParameter(Name = "role", Description = "Role to use for reaction")]
        Role role)
    {
        bool success = await reactionRoleManager.AddAsync(reactEmoji, role);
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
}