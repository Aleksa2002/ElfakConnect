using System;
using MongoDB.Bson;
using Server.Common;

namespace Server.Authentication.Interfaces;

public interface IAccessTokenService
{
    Task GenerateBothTokensAndSetCookies(User user);
    Task<Result<User>> VerifyAndRevokeRefreshToken(string refreshToken);
    Task RevokeAllUserRefreshTokens(ObjectId userId);
}