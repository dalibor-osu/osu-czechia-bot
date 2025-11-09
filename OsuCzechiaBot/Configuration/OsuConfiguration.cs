using OsuCzechiaBot.Exceptions;

namespace OsuCzechiaBot.Configuration;

public class OsuConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new ConfigurationException("ClientId is not configured");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new ConfigurationException("ClientSecret is not configured");
        }
    }
}