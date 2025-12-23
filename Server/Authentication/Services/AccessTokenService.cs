using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using Server.Common.Api;
using Server.Data.Repositories;

namespace Server.Authentication.Services;

public class JwtOptions
{
    public required string Secret { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int AccessTokenLifeTimeMinutes { get; set; }
    public int RefreshTokenLifeTimeHours { get; set; }
}

public class AccessTokenService(
    IOptions<JwtOptions> jwtOptions,
    IHttpContextAccessor httpContextAccessor,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository
) : IAccessTokenService
{

    public async Task GenerateBothTokensAndSetCookies(User user)
    {
        var (accessToken, accessTokenExpiresAtUtc) = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddHours(jwtOptions.Value.RefreshTokenLifeTimeHours);

        await refreshTokenRepository.CreateAsync(
            user.Id,
            refreshToken,
            refreshTokenExpiresAtUtc
        );

        WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", accessToken, accessTokenExpiresAtUtc);
        WriteAuthTokenAsHttpOnlyCookie("REFRESH_TOKEN", refreshToken, refreshTokenExpiresAtUtc);
    }

    public async Task<Result<User>> VerifyAndRevokeRefreshToken(string refreshToken)
    {
        var storedToken = await refreshTokenRepository.FindByTokenAsync(refreshToken);
        if (storedToken == null)
        {
            return AuthErrors.InvalidRefreshToken;
        }
        if (storedToken.IsRevoked)
        {
            await RevokeAllUserRefreshTokens(storedToken.UserId);
            return AuthErrors.InvalidRefreshToken;
        }
        
        await refreshTokenRepository.RevokeByIdAsync(storedToken.Id);

        if (storedToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return AuthErrors.ExpiredRefreshToken;
        }

        var user = await userRepository.FindByIdAsync(storedToken.UserId);
        
        if (user == null)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        return user;
    }
    public async Task RevokeAllUserRefreshTokens(ObjectId userId)
    {
        await refreshTokenRepository.DeleteAllByUserIdAsync(userId);
    }
    private (string jwtToken, DateTime expiresAtUtc) GenerateAccessToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOptions.Value.Secret));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.EmailVerified, user.IsVerified.ToString().ToLower())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenLifeTimeMinutes),
            Issuer = jwtOptions.Value.Issuer,
            Audience = jwtOptions.Value.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (token, tokenDescriptor.Expires.Value);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token,
        DateTime expiration)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName,
            token, new CookieOptions
            {
                HttpOnly = true,
                Expires = expiration,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
    }
}