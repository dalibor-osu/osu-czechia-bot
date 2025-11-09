using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class Kudosu
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("available")]
    public int Available { get; set; }
}