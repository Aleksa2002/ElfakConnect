using System;
using MongoDB.Bson;
using Server.Common.Api;

namespace Server.Data.Repositories;

public interface IVerificationTokenRepository
{
    Task<VerificationToken> CreateAsync(ObjectId userId, string code, DateTime expiresAtUtc);
    Task<VerificationToken?> FindByCodeAndUserIdAsync(string code, string userId);
    Task DeleteByIdAsync(ObjectId id);
}
