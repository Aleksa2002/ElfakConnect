using System;
using Server.Common;

namespace Server.Users;

public static class UserErrors
{
    public static Error UsernameNotUnique(string username) =>
        Error.Conflict(ErrorCodes.RES_CONFLICT_USERNAME,"User.UsernameNotUnique", $"A user with the username '{username}' already exists.");
    public static Error EmailNotUnique(string email) =>
        Error.Conflict(ErrorCodes.RES_CONFLICT_EMAIL,"User.EmailNotUnique", $"A user with the email '{email}' already exists.");
    public static Error MicrosoftIdNotUnique(string microsoftId) =>
        Error.Conflict(ErrorCodes.RES_CONFLICT, "User.MicrosoftIdNotUnique", $"A user with the Microsoft ID '{microsoftId}' already exists.");
    public static Error UserNotFound(string userId) =>
        Error.NotFound(ErrorCodes.RES_NOT_FOUND, "User.UserNotFound", $"No user found with ID '{userId}'.");
}
