using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Data.Interfaces;

namespace Server.Data.Repositories;

public class RefreshTokenRepository(IMongoDatabase database) : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> refreshTokenCollection =
         database.GetCollection<RefreshToken>(RefreshToken.CollectionName);
    public async Task<RefreshToken> CreateAsync(ObjectId userId, string token, DateTime expiresAtUtc)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            ExpiresAtUtc = expiresAtUtc,
            TokenHash = Convert.ToBase64String(hash)
        };
        await refreshTokenCollection.InsertOneAsync(refreshToken);
        
        return refreshToken;
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        var tokenHash = Convert.ToBase64String(hash);
        
        return await refreshTokenCollection
            .Find(rt => rt.TokenHash == tokenHash)
            .FirstOrDefaultAsync();
    }

    public async Task DeleteAllByUserIdAsync(ObjectId userId)
    {
        await refreshTokenCollection.DeleteManyAsync(rt => rt.UserId == userId);
    }

    public async Task RevokeByIdAsync(ObjectId id)
    {        
        await refreshTokenCollection.UpdateOneAsync(
            rt => rt.Id == id,
            Builders<RefreshToken>.Update.Set(rt => rt.IsRevoked, true));
    }
}
