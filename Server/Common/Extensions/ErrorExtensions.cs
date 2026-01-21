using System;
using Microsoft.AspNetCore.Mvc;

namespace Server.Common.Extensions;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error)
    {
        return new ProblemDetails
        {
            Type = GetType(error.Status),
            Title = error.Title,
            Status = error.Status,
            Detail = error.Message,
            Extensions =
            { ["errorCode"] = error.Code }
        };  
    }
    private static string GetType(int status) => status switch
    {
        StatusCodes.Status404NotFound => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
        StatusCodes.Status409Conflict => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
        StatusCodes.Status401Unauthorized => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.2",
        _ => "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
    };
}
