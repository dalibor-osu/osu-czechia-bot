using System.ComponentModel.DataAnnotations;
using OsuCzechiaBot.Models.Interfaces;

namespace OsuCzechiaBot.Models;

public class ApplicationSetting : IIdentifiable<string>
{
    [Key]
    [MaxLength(256)]
    public required string Id { get; set; }

    [MaxLength(1024)]
    public required string Value { get; set; }
}