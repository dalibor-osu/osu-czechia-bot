namespace OsuCzechiaBot.Helpers;

public class EmojiReferencePair
{
    public required string Name { get; set; }
    public required ulong Id { get; set; }
    public required bool IsAnimated { get; set; }

    public override string ToString()
    {
        string prefix = IsAnimated ? "<a" : "<";
        return $"{prefix}:{Name}:{Id}>";
    }
}