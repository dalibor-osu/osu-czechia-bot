namespace OsuCzechiaBot.Helpers.Optionals;

public record Error
{
    public required ErrorType ErrorType { get; init; }
    public required string Message { get; init; }
}