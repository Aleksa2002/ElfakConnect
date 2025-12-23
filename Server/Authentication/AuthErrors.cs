using System;
using Server.Common.Api;

namespace Server.Authentication;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "The provided credentials are invalid.");
    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized("Auth.InvalidRefreshToken", "The provided refresh token is invalid.");
    public static readonly Error MissingRefreshToken =
        Error.Unauthorized("Auth.MissingRefreshToken", "The refresh token is missing.");
    public static readonly Error ExpiredRefreshToken =
        Error.Unauthorized("Auth.ExpiredRefreshToken", "The refresh token has expired.");
    public static readonly Error InvalidAccessToken =
        Error.Unauthorized("Auth.InvalidAccessToken", "The access token is invalid.");
    public static readonly Error InvalidVerificationCode =
        Error.Validation("Auth.InvalidVerificationCode", "The verification code is invalid.");
    public static readonly Error ExpiredVerificationCode =
        Error.Validation("Auth.ExpiredVerificationCode", "The verification code has expired.");
}
