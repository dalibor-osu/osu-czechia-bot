using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class TokenFromRefreshTokenRequest : TokenRequestBase
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [JsonPropertyName("grant_type")]
    public string GrantType => "refresh_token";
}