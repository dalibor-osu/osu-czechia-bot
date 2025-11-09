using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class TokenFromCodeRequest
{
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;
    
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = string.Empty;
    
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("grant_type")]
    public string GrantType => "authorization_code";
}