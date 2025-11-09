using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public abstract class TokenRequestBase
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;
    
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = string.Empty;
}