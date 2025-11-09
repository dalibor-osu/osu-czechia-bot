using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class OsuUserExtendedWithOptionalData : OsuUserExtended
{
    [JsonPropertyName("country")]
    public Country? Country { get; set; }
    
    [JsonPropertyName("cover")]
    public Cover? Cover { get; set; }
    
    [JsonPropertyName("is_restricted")]
    public bool IsRestricted { get; set; }
    
    [JsonPropertyName("kudosu")]
    public Kudosu Kudosu { get; set; } = null!;
    
    [JsonPropertyName("statistics_rulesets")]
    public RulesetsStatistics? RulesetsStatistics { get; set; } = null!;
}