using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class TokenFromCodeRequest : TokenRequestBase
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;
    
    [JsonPropertyName("grant_type")]
    public string GrantType => "authorization_code";
}