using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class MainRulesetStatistics : RulesetStatistics
{
    /// <summary>
    /// Country rank.
    /// </summary>
    [JsonPropertyName("country_rank")]
    public ulong? CountryRank { get; set; }
}