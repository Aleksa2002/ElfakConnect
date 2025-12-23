using System;
using MongoDB.Bson;

namespace Server.Data.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(ObjectId userId, string token, DateTime expiresAtUtc);
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task DeleteAllByUserIdAsync(ObjectId userId);
    Task RevokeByIdAsync(ObjectId id);
}
