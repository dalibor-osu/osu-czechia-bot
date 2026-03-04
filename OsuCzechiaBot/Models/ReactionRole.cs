using System.ComponentModel.DataAnnotations;
using NetCord.Rest;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Models;

public class ReactionRole : IIdentifiable<long>
{
    [Key]
    public long Id { get; set; }
    
    public required ulong RoleId { get; set; }
    [MaxLength(32)]
    public string? EmojiName { get; set; }
    public ulong? EmojiId { get; set; }
    public bool IsAnimated { get; set; }
    public bool IsUnicode { get; set; }
    [MaxLength(8)]
    public string? UnicodeValue { get; set; }
    
    [MaxLength(256)]
    public string? Description { get; set; }

    public string GetRoleString() => $"<@&{RoleId}>";

    public string GetEmojiString()
    {
        if (IsUnicode)
        {
            return UnicodeValue!;
        }
        
        string prefix = IsAnimated ? "<a" : "<";
        return $"{prefix}:{EmojiName}:{EmojiId}>";
    }

    public ReactionEmojiProperties ToEmojiProperties()
    {
        if (IsUnicode)
        {
            return new ReactionEmojiProperties($"{UnicodeValue}");
        }
        
        return new ReactionEmojiProperties(EmojiName!, EmojiId!.Value);
    }
}