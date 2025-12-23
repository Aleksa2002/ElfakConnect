using System;
using MongoDB.Bson;
using Server.Common.Api;

namespace Server.Authentication.Services;

public interface IAccessTokenService
{
    Task GenerateBothTokensAndSetCookies(User user);
    Task<Result<User>> VerifyAndRevokeRefreshToken(string refreshToken);
    Task RevokeAllUserRefreshTokens(ObjectId userId);
}