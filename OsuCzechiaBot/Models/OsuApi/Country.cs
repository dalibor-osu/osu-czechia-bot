using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class Country
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}