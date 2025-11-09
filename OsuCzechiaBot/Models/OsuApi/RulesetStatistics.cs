using System.Text.Json.Serialization;

namespace OsuCzechiaBot.Models.OsuApi;

public class RulesetStatistics
{
   /// <summary>
    /// Number of 100 hits.
    /// </summary>
    [JsonPropertyName("count_100")]
    public int Count100 { get; set; }

    /// <summary>
    /// Number of 300 hits.
    /// </summary>
    [JsonPropertyName("count_300")]
    public int Count300 { get; set; }

    /// <summary>
    /// Number of 50 hits.
    /// </summary>
    [JsonPropertyName("count_50")]
    public int Count50 { get; set; }

    /// <summary>
    /// Number of misses.
    /// </summary>
    [JsonPropertyName("count_miss")]
    public int CountMiss { get; set; }

    /// <summary>
    /// Player level info.
    /// </summary>
    [JsonPropertyName("level")]
    public Level Level { get; set; } = null!;

    /// <summary>
    /// Global rank.
    /// </summary>
    [JsonPropertyName("global_rank")]
    public ulong? GlobalRank { get; set; }

    /// <summary>
    /// Global rank as a percent (0–1).
    /// </summary>
    [JsonPropertyName("global_rank_percent")]
    public double? GlobalRankPercent { get; set; }

    /// <summary>
    /// Global rank experience (nullable).
    /// </summary>
    [JsonPropertyName("global_rank_exp")]
    public double? GlobalRankExp { get; set; }

    /// <summary>
    /// Performance points.
    /// </summary>
    [JsonPropertyName("pp")]
    public double Pp { get; set; }

    /// <summary>
    /// Performance points experience.
    /// </summary>
    [JsonPropertyName("pp_exp")]
    public double PpExp { get; set; }

    /// <summary>
    /// Ranked score.
    /// </summary>
    [JsonPropertyName("ranked_score")]
    public long RankedScore { get; set; }

    /// <summary>
    /// Hit accuracy in percentage.
    /// </summary>
    [JsonPropertyName("hit_accuracy")]
    public double HitAccuracy { get; set; }

    /// <summary>
    /// Total play count.
    /// </summary>
    [JsonPropertyName("play_count")]
    public int PlayCount { get; set; }

    /// <summary>
    /// Total play time in seconds.
    /// </summary>
    [JsonPropertyName("play_time")]
    public int PlayTime { get; set; }

    /// <summary>
    /// Total score.
    /// </summary>
    [JsonPropertyName("total_score")]
    public long TotalScore { get; set; }

    /// <summary>
    /// Total hits.
    /// </summary>
    [JsonPropertyName("total_hits")]
    public int TotalHits { get; set; }

    /// <summary>
    /// Maximum combo achieved.
    /// </summary>
    [JsonPropertyName("maximum_combo")]
    public int MaximumCombo { get; set; }

    /// <summary>
    /// Replays watched by others.
    /// </summary>
    [JsonPropertyName("replays_watched_by_others")]
    public int ReplaysWatchedByOthers { get; set; }

    /// <summary>
    /// Whether the player is ranked.
    /// </summary>
    [JsonPropertyName("is_ranked")]
    public bool IsRanked { get; set; }

    /// <summary>
    /// Grade counts.
    /// </summary>
    [JsonPropertyName("grade_counts")]
    public GradeCounts GradeCounts { get; set; } = null!; 
}