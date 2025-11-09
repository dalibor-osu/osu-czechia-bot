using System.ComponentModel.DataAnnotations;

namespace OsuCzechiaBot.Models;

public class AuthorizedUser
{
    [Key]
    public ulong DiscordId { get; set; }
    
    [Required]
    public int OsuId { get; set; }

    [Required]
    [MaxLength(1024)]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1024)]
    public string RefreshToken { get; set; } = string.Empty;
    
    [Required]
    public DateTimeOffset Expires { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CountryCode { get; set; } = string.Empty;
}