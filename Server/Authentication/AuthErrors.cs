using System;
using Server.Common;

namespace Server.Authentication;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized(ErrorCodes.AUTH_INVALID_CREDENTIALS, "Auth.InvalidCredentials", "The provided credentials are invalid.");
    public static readonly Error InvalidLoginMethod =
        Error.Unauthorized(ErrorCodes.AUTH_INVALID_LOGIN_METHOD, "Auth.InvalidLoginMethod", "The login method is invalid.");
    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized(ErrorCodes.AUTH_EXPIRED, "Auth.InvalidRefreshToken", "The refresh token is invalid.");
    public static readonly Error MissingRefreshToken =
        Error.Unauthorized(ErrorCodes.AUTH_EXPIRED, "Auth.MissingRefreshToken", "The refresh token is missing.");
    public static readonly Error ExpiredRefreshToken =
        Error.Unauthorized(ErrorCodes.AUTH_EXPIRED, "Auth.ExpiredRefreshToken", "The refresh token has expired.");
    public static readonly Error InvalidVerificationCode =
        Error.Validation(ErrorCodes.VAL_FAILED, "Auth.InvalidVerificationCode", "The verification code is invalid.");
    public static readonly Error ExpiredVerificationCode =
        Error.Validation(ErrorCodes.VAL_FAILED, "Auth.ExpiredVerificationCode", "The verification code has expired.");
}

