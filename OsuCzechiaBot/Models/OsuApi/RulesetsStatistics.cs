using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class RulesetsStatistics
{
    [JsonPropertyName("osu")]
    public RulesetStatistics? Osu { get; set; } = null;
    
    [JsonPropertyName("taiko")]
    public RulesetStatistics? Taiko { get; set; } = null;
    
    [JsonPropertyName("fruits")]
    public RulesetStatistics? Fruits { get; set; } = null;
    
    [JsonPropertyName("mania")]
    public RulesetStatistics? Mania { get; set; } = null;
    
}