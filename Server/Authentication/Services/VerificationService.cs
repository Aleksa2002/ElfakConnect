using System;
using MongoDB.Bson;
using Server.Common;
using Server.Data.Interfaces;
using Server.Authentication.Interfaces;

namespace Server.Authentication.Services;

public class VerificationService(
    IUserRepository userRepository,
    IVerificationTokenRepository verificationTokenRepository,
    IAccessTokenService accessTokenService) : IVerificationService
{
    public async Task<string> CreateCode(ObjectId userId)
    {
        string code = new Random().Next(100000, 999999).ToString();

        await verificationTokenRepository.CreateAsync(
            userId,
            code,
            DateTime.UtcNow.AddHours(24)
        );
        return code;
    }

    public async Task<Result<User>> VerifyUser(string userId, string code)
    {
        var token = await verificationTokenRepository.FindByCodeAndUserIdAsync(code, userId);
        if (token == null)
        {
            return AuthErrors.InvalidVerificationCode;
        }
        if (token.ExpiresAtUtc < DateTime.UtcNow)
        {
            return AuthErrors.ExpiredVerificationCode;
        }
        var updatedUserResult = await userRepository.VerifyByIdAsync(userId);

        if (updatedUserResult.IsFailure)
        {
            return updatedUserResult;
        }

        var user = updatedUserResult.Value;
        
        await Task.WhenAll(
                    verificationTokenRepository.DeleteByIdAsync(token.Id),
                    accessTokenService.RevokeAllUserRefreshTokens(user.Id));
        await accessTokenService.GenerateBothTokensAndSetCookies(user);

        return user;
    }
}
