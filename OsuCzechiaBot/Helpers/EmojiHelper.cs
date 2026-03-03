using System.Diagnostics.CodeAnalysis;

namespace OsuCzechiaBot.Helpers;

public static class EmojiHelper
{
    public static bool TryParse(string input, [NotNullWhen(true)] out EmojiReferencePair? emoji)
    {
        emoji = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        if ((!input.StartsWith("<:") && !input.StartsWith("<a:")) || !input.EndsWith('>'))
        {
            return false;
        }

        string[] split = input.Split(':');
        if (split.Length != 3)
        {
            return false;
        }

        string name = split[1];
        if (!ulong.TryParse(split[2].TrimEnd('>'), out ulong id))
        {
            return false;
        }
        
        emoji = new EmojiReferencePair { Name = name, Id = id, IsAnimated = split[0] == "<a"};
        return true;
    }
}