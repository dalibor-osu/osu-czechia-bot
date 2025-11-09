using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using OsuCzechiaBot.Configuration;

namespace OsuCzechiaBot.Handlers;

public class MessageCreateHandler(ConfigurationAccessor configurationAccessor) : IMessageCreateGatewayHandler
{
    public async ValueTask HandleAsync(Message arg)
    {
        if (arg.ChannelId != configurationAccessor.Discord.AuthChannelId || arg.Author.IsBot)
        {
            return;
        }

        if (arg.Author is GuildUser guildUser)
        {
            var roles = guildUser.GetRoles(arg.Guild!).ToList();
            if (roles.Any(r => r.Permissions.HasFlag(Permissions.Administrator)) || arg.Guild?.OwnerId == guildUser.Id)
            {
                return;
            }
        }

        await arg.DeleteAsync();
    }
}