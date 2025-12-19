namespace Server.Common.Api;

public enum ErrorType
{
    Validation = 0,
    NotFound = 1,
    Conflict = 2,
    Unauthorized = 3,
}
public record Error
{
    private Error(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        Type = errorType;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);
}

