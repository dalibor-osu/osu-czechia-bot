namespace OsuCzechiaBot.Helpers.Optionals;

public record Optional<T>
{
    private readonly T? _value;
    private readonly Error? _error;
    public bool Success { get; init; }

    public T Value => !Success ? throw new InvalidOperationException("Tried accessing value in error optional") : _value!;
    public Error Error => Success ? throw new InvalidOperationException("Tried accessing error in success optional") : _error!;

    public Optional(T value)
    {
        _value = value;
        _error = null;
        Success = true;
    }

    public Optional(Error error)
    {
        _error = error;
        _value = default;
        Success = false;
    }
    
    public static implicit operator Optional<T>(T value) => new (value);
    public static implicit operator Optional<T>(Error error) => new (error);
}