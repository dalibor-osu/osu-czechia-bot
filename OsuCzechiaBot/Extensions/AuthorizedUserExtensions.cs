using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Extensions;

public static class AuthorizedUserExtensions
{
    public static string GetOsuProfileLink(this AuthorizedUser user) => $"https://osu.ppy.sh/users/{user.OsuId}";
    public static string GetMarkdownOsuProfileLink(this AuthorizedUser user) => $"[osu! profile]({user.GetOsuProfileLink()})";
}