using System;
using Server.Common.Api;

namespace Server.Authentication.Services;

public interface IAuthenticationService
{
    Task<Result<User>> Login(string username, string password, CancellationToken cancellationToken);
    Task<Result<User>> Register(string username, string password, string email, CancellationToken cancellationToken);
    Task<Result> RefreshAccessTokens(string? refreshToken, CancellationToken cancellationToken);
}
