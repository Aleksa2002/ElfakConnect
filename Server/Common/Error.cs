using Microsoft.AspNetCore.Mvc;
using Server.Common.Extensions;

namespace Server.Common;

public record Error
{
    public int Status { get; }
    public string Title { get; }
    public string Message { get; }
    public string Code { get; }

    private Error(string code, string title, string message, int status)
    {
        Status = status;
        Title = title;;
        Message = message;
        Code = code;
    }
    public static readonly Error None = new(string.Empty, string.Empty, string.Empty, StatusCodes.Status400BadRequest);
    public static Error Validation(string code, string title, string message) =>
        new(code, title, message, StatusCodes.Status400BadRequest);
    public static Error NotFound(string code, string title, string message) =>
        new(code, title, message, StatusCodes.Status404NotFound);
    public static Error Conflict(string code, string title, string message) =>
        new(code, title, message, StatusCodes.Status409Conflict);
    public static Error  Unauthorized(string code, string title, string message) =>
        new(code, title, message, StatusCodes.Status401Unauthorized);

    public static implicit operator ProblemDetails(Error error) => error.ToProblemDetails();
}

public static class ErrorCodes
{
    public const string AUTH_INVALID_CREDENTIALS = "AUTH_001";
    public const string AUTH_INVALID_LOGIN_METHOD = "AUTH_002";
    public const string AUTH_EXPIRED = "AUTH_003";

    public const string VAL_FAILED = "VAL_001";

    public const string RES_NOT_FOUND = "RES_001";
    public const string RES_CONFLICT = "RES_002";
    public const string RES_CONFLICT_EMAIL = "RES_003";
    public const string RES_CONFLICT_USERNAME = "RES_004";

    public const string SYS_INTERNAL_ERROR = "SYS_001";
}
