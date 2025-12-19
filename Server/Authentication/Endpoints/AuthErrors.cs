using System;
using Server.Common.Api;

namespace Server.Authentication.Endpoints;

public static class AuthErrors
{
    public static Error UsernameNotUnique(string username) =>
        Error.Conflict("Auth.UsernameNotUnique", $"A user with the username '{username}' already exists.");

    public static Error EmailNotUnique(string email) =>
        Error.Conflict("Auth.EmailNotUnique", $"A user with the email '{email}' already exists.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "The provided credentials are invalid.");
}
