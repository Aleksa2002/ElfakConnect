using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Common.Extensions;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error)
    {

        return new ProblemDetails
        {
            Type = GetType(error.Type),
            Title = GetTitle(error.Type),
            Status = GetStatus(error.Type),
            Detail = error.Description,
            Extensions =
            { ["code"] = error.Code }
        };
        
    }
    private static int GetStatus(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError,
    };

    private static string GetType(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
        ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
        ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2",
        _ => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
    };

    private static string GetTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.NotFound => "Resource Not Found",
        ErrorType.Conflict => "Conflict Occurred",
        ErrorType.Unauthorized => "Unauthorized Access",
        _ => "Internal Server Error",
    };
}
