using System.ComponentModel.DataAnnotations;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Models;

public class AuthorizedUser : IIdentifiable
{
    [Key]
    public ulong Id { get; set; }
    
    [Required]
    public int OsuId { get; set; }

    [Required]
    [MaxLength(1024)]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(1024)]
    public string RefreshToken { get; set; } = string.Empty;
    
    [Required]
    public DateTimeOffset? Expires { get; set; }
    
    [Required]
    [MaxLength(3)]
    public string CountryCode { get; set; } = string.Empty;

    public bool Authorized { get; set; } = true;
}