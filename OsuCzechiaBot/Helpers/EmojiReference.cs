using NetCord;
using OsuCzechiaBot.Models;

namespace OsuCzechiaBot.Helpers;

public class EmojiReference
{
    public string? Name { get; private set; }
    public ulong? Id { get; private set; }
    public bool IsAnimated { get; private set; }
    public bool IsUnicode { get; private set; }
    public string? UnicodeValue { get; private set; }

    public EmojiReference(string unicode)
    {
        UnicodeValue = unicode;
        IsUnicode = true;
        IsAnimated = false;
    }

    public EmojiReference(string name, ulong id, bool isAnimated)
    {
        Name = name;
        Id = id;
        IsAnimated = isAnimated;
        IsUnicode = false;
    }

    public ReactionRole ToReactionRole(Role role)
    {
        return new ReactionRole
        {
            RoleId = role.Id,
            EmojiId = Id,
            EmojiName = Name,
            IsAnimated = IsAnimated,
            IsUnicode = IsUnicode,
            UnicodeValue = UnicodeValue
        };
    }

    public override string ToString()
    {
        if (IsUnicode)
        {
            return UnicodeValue!;
        }

        string prefix = IsAnimated ? "<a" : "<";
        return $"{prefix}:{Name}:{Id}>";
    }
}