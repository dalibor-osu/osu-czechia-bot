using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class Level
{
    /// <summary>
    /// Current level.
    /// </summary>
    [JsonPropertyName("current")]
    public int Current { get; set; }

    /// <summary>
    /// Progress to next level in percent.
    /// </summary>
    [JsonPropertyName("progress")]
    public int Progress { get; set; }
}