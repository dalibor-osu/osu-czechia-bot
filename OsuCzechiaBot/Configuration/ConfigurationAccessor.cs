namespace OsuCzechiaBot.Configuration;

public class ConfigurationAccessor
{
    public DiscordConfiguration Discord { get; set; } = new();
    public OsuConfiguration Osu { get; set; } = new();

    public ConfigurationAccessor(IConfiguration configuration)
    {
        foreach (var property in GetType().GetProperties())
        {
            configuration.GetSection(property.Name).Bind(property.GetValue(this));
        }

        Discord.Validate();
        Osu.Validate();
    }
}