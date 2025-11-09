using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class OsuUserExtended : OsuUser
{
    /// <summary>
    /// URL of profile cover. Deprecated, use cover.url instead.
    /// </summary>
    [JsonPropertyName("cover_url")]
    public string CoverUrl { get; set; } = null!;

    /// <summary>
    /// Discord username (nullable).
    /// </summary>
    [JsonPropertyName("discord")]
    public string? Discord { get; set; }

    /// <summary>
    /// Whether the user has a current or past osu!supporter tag.
    /// </summary>
    [JsonPropertyName("has_supported")]
    public bool HasSupported { get; set; }

    /// <summary>
    /// User interests (nullable).
    /// </summary>
    [JsonPropertyName("interests")]
    public string? Interests { get; set; }

    /// <summary>
    /// Join date of the user.
    /// </summary>
    [JsonPropertyName("join_date")]
    public DateTimeOffset JoinDate { get; set; }

    /// <summary>
    /// User location (nullable).
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>
    /// Maximum number of users allowed to be blocked.
    /// </summary>
    [JsonPropertyName("max_blocks")]
    public int MaxBlocks { get; set; }

    /// <summary>
    /// Maximum number of friends allowed to be added.
    /// </summary>
    [JsonPropertyName("max_friends")]
    public int MaxFriends { get; set; }

    /// <summary>
    /// User occupation (nullable).
    /// </summary>
    [JsonPropertyName("occupation")]
    public string? Occupation { get; set; }

    /// <summary>
    /// Ruleset the user primarily plays (e.g., "osu").
    /// </summary>
    [JsonPropertyName("playmode")]
    public string Playmode { get; set; } = null!;

    /// <summary>
    /// Device choices of the user.
    /// </summary>
    [JsonPropertyName("playstyle")]
    public string[] Playstyle { get; set; } = [];

    /// <summary>
    /// Number of forum posts.
    /// </summary>
    [JsonPropertyName("post_count")]
    public int PostCount { get; set; }

    /// <summary>
    /// Custom colour hue in HSL degrees (0–359). Nullable.
    /// </summary>
    [JsonPropertyName("profile_hue")]
    public int? ProfileHue { get; set; }

    /// <summary>
    /// Ordered array of sections in user profile page.
    /// </summary>
    [JsonPropertyName("profile_order")]
    public string[] ProfileOrder { get; set; } = [];

    /// <summary>
    /// User-specific title (nullable).
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// URL for the title (nullable).
    /// </summary>
    [JsonPropertyName("title_url")]
    public string? TitleUrl { get; set; }

    /// <summary>
    /// Twitter username (nullable).
    /// </summary>
    [JsonPropertyName("twitter")]
    public string? Twitter { get; set; }

    /// <summary>
    /// Website URL (nullable).
    /// </summary>
    [JsonPropertyName("website")]
    public string? Website { get; set; }
}