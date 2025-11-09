using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class OsuUser
{
    /// <summary>
    /// URL of user's avatar.
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; } = null!;

    /// <summary>
    /// Two-letter code representing user's country.
    /// </summary>
    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; } = null!;

    /// <summary>
    /// Identifier of the default Group the user belongs to.
    /// </summary>
    [JsonPropertyName("default_group")]
    public string? DefaultGroup { get; set; }

    /// <summary>
    /// Unique identifier for user.
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// Has this account been active in the last x months?
    /// </summary>
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Is this a bot account?
    /// </summary>
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    /// <summary>
    /// Indicates whether the account is deleted.
    /// </summary>
    [JsonPropertyName("is_deleted")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Is the user currently online? (either on lazer or the new website)
    /// </summary>
    [JsonPropertyName("is_online")]
    public bool IsOnline { get; set; }

    /// <summary>
    /// Does this user have supporter?
    /// </summary>
    [JsonPropertyName("is_supporter")]
    public bool IsSupporter { get; set; }

    /// <summary>
    /// Last access time. Null if the user hides online presence.
    /// </summary>
    [JsonPropertyName("last_visit")]
    public DateTimeOffset? LastVisit { get; set; }

    /// <summary>
    /// Whether or not the user allows PM from other than friends.
    /// </summary>
    [JsonPropertyName("pm_friends_only")]
    public bool PmFriendsOnly { get; set; }

    /// <summary>
    /// Colour of username/profile highlight, hex code (e.g. #333333)
    /// </summary>
    [JsonPropertyName("profile_colour")]
    public string? ProfileColour { get; set; }

    /// <summary>
    /// User's display name.
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;
}
