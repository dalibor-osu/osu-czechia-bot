namespace OsuCzechiaBot.Helpers.Optionals;

public enum ErrorType
{
    Unknown = 0,
    
    // Response errors
    BadRequest = 10,
    NotFound,
    ServiceError,
    Forbidden,
    
    // Internal errors
    Null = 20,
    InvalidValue
}