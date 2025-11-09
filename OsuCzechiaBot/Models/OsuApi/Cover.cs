using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class Cover
{
    [JsonPropertyName("custom_url")]
    public string CustomUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}