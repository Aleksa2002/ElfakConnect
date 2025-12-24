using System;
using Server.Common;

namespace Server.Users;

public static class UserErrors
{
    public static Error UsernameNotUnique(string username) =>
        Error.Conflict("Auth.UsernameNotUnique", $"A user with the username '{username}' already exists.");
    public static Error EmailNotUnique(string email) =>
        Error.Conflict("Auth.EmailNotUnique", $"A user with the email '{email}' already exists.");
    public static Error UserNotFound(string userId) =>
        Error.NotFound("Auth.UserNotFound", $"No user found with ID '{userId}'.");
}
