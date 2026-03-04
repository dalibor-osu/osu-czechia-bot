using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OsuCzechiaBot.Helpers;

public static class EmojiHelper
{
    public static bool TryParse(string input, [NotNullWhen(true)] out EmojiReference? emoji)
    {
        if (IsSingleEmoji(input))
        {
            emoji = new EmojiReference(input);
            return true;
        }

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

        emoji = new EmojiReference(name, id, split[0] == "<a");
        return true;
    }

    // Vibecoded random method. Surely works.
    private static bool IsSingleEmoji(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        int i = 0;
        int runeCount = 0;

        while (i < input.Length)
        {
            if (!Rune.TryGetRuneAt(input, i, out var rune))
            {
                return false;
            }

            runeCount++;

            if (!IsEmojiRune(rune))
            {
                return false;
            }

            i += rune.Utf16SequenceLength;

            // Handle variation selector (FE0F)
            if (i < input.Length && Rune.TryGetRuneAt(input, i, out var vs) && vs.Value == 0xFE0F)
            {
                i += vs.Utf16SequenceLength;
            }

            // Handle ZWJ sequences
            if (i < input.Length && Rune.TryGetRuneAt(input, i, out var zwj) && zwj.Value == 0x200D)
            {
                i += zwj.Utf16SequenceLength;
            }
        }

        return runeCount > 0;
    }

    private static bool IsEmojiRune(Rune rune)
    {
        int value = rune.Value;
        return
            value is >= 0x1F600 and <= 0x1F64F || // Emoticons
            value is >= 0x1F300 and <= 0x1F5FF || // Misc Symbols & Pictographs
            value is >= 0x1F680 and <= 0x1F6FF || // Transport & Map
            value is >= 0x2600 and <= 0x26FF || // Misc symbols
            value is >= 0x2700 and <= 0x27BF || // Dingbats
            value is >= 0x1F1E6 and <= 0x1F1FF; // Flags (regional indicators)
    }
}