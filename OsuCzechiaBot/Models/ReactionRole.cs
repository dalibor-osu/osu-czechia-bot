using System.ComponentModel.DataAnnotations;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Models;

public class ReactionRole : IIdentifiable<long>
{
    [Key]
    public long Id { get; set; }
    
    public required ulong RoleId { get; set; }
    [MaxLength(32)]
    public required string EmojiName { get; set; }
    public required ulong EmojiId { get; set; }
    public required bool IsAnimated { get; set; }

    public string GetRoleString() => $"<@&{RoleId}>";

    public string GetEmojiString()
    {
        string prefix = IsAnimated ? "<a" : "<";
        return $"{prefix}:{EmojiName}:{EmojiId}>";
    }
}