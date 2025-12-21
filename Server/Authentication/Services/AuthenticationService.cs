using System;
using MongoDB.Driver;
using Server.Authentication.Endpoints;
using Server.Common.Api;
using Server.Common.Api.Extensions;

namespace Server.Authentication.Services;

public class AuthenticationService(
    IAccessTokenProvider jwtTokenProvider,
    IPasswordHasher passwordHasher,
    IMongoDatabase database
) : IAuthenticationService
{
    public async Task<Result<User>> Register(string username, string password, string email, CancellationToken cancellationToken)
    {
        var usersCollection = database.GetCollection<User>(User.CollectionName);

        var emailExists = await usersCollection
            .Find(x => x.Email == email)
            .AnyAsync(cancellationToken);

        if (emailExists)
        {
            return AuthErrors.EmailNotUnique(email);
        }    
        
        var usernameExists = await usersCollection
            .Find(x => x.Username == username)
            .AnyAsync(cancellationToken);

        if (usernameExists)
        {
            return AuthErrors.UsernameNotUnique(username);
        }    

        var newUser = new User
        {
            Username = username,
            PasswordHash = passwordHasher.Hash(password),
            Email = email
        };
        
        await usersCollection.InsertOneAsync(newUser, cancellationToken: cancellationToken);
        return newUser;
    }
    public async Task<Result<User>> Login(string email, string password, CancellationToken cancellationToken)
    {
        var usersCollection = database.GetCollection<User>(User.CollectionName);

        var user = await usersCollection
            .Find(x => x.Email == email)
            .FirstAsync(cancellationToken);

        if (user == null || !passwordHasher.Verify(password, user.PasswordHash))
        {
            return AuthErrors.InvalidCredentials;
        }

        var (jwtToken, expiresAtUtc) = jwtTokenProvider.GenerateJwtToken(user);
        var refreshToken = jwtTokenProvider.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        await usersCollection.UpdateOneAsync(
            x => x.Id == user.Id,
            Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiresAtUtc, refreshTokenExpiresAtUtc),
            cancellationToken: cancellationToken);

        jwtTokenProvider.WriteAuthTokenAsHttpOnlyCookie(
            "ACCESS_TOKEN",
            jwtToken,
            expiresAtUtc);

        jwtTokenProvider.WriteAuthTokenAsHttpOnlyCookie(
            "REFRESH_TOKEN",
            refreshToken,
            refreshTokenExpiresAtUtc);

        return user;
    }

    public async Task<Result> RefreshAccessTokens(string? refreshToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            return AuthErrors.InvalidRefreshToken;
        }
        var usersCollection = database.GetCollection<User>(User.CollectionName);
        var user = await usersCollection
            .Find(x => x.RefreshToken == refreshToken)
            .FirstAsync(cancellationToken);

        if (user == null || user.RefreshTokenExpiresAtUtc < DateTime.UtcNow)
        {
            return AuthErrors.InvalidRefreshToken;
        }
        
        var (jwtToken, expiresAtUtc) = jwtTokenProvider.GenerateJwtToken(user);
        refreshToken = jwtTokenProvider.GenerateRefreshToken();

        var refreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7);

        await usersCollection.UpdateOneAsync(
            x => x.Id == user.Id,
            Builders<User>.Update
                .Set(u => u.RefreshToken, refreshToken)
                .Set(u => u.RefreshTokenExpiresAtUtc, refreshTokenExpiresAtUtc),
            cancellationToken: cancellationToken);

        jwtTokenProvider.WriteAuthTokenAsHttpOnlyCookie(
            "ACCESS_TOKEN",
            jwtToken,
            expiresAtUtc);

        jwtTokenProvider.WriteAuthTokenAsHttpOnlyCookie(
            "REFRESH_TOKEN",
            refreshToken,
            refreshTokenExpiresAtUtc);

        return Result.Success();
    }

}
