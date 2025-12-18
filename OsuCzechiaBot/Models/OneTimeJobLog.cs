using System.ComponentModel.DataAnnotations;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Models;

public record OneTimeJobLog : IIdentifiable<string>
{
    [Key]
    public required string Id { get; set; }

    [Required]
    public DateTimeOffset StartTime { get; set; }
}