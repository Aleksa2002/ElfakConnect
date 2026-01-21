using System;

namespace Server.Common;

public static class ApiResponse
{
    public static IResult Ok<T>(T data, int status = 200) 
        => Results.Ok(new { data, status = 200, success = true });

    public static IResult Ok(int status = 200) 
        => Results.Ok(new { data = (object?)null, status, success = true });
    
    public static IResult Problem(Error error) 
        => Results.Problem(error);
}
