using System;
using MongoDB.Bson;
using Server.Common;

namespace Server.Data.Interfaces;

public interface IVerificationTokenRepository
{
    Task<VerificationToken> CreateAsync(ObjectId userId, string code, DateTime expiresAtUtc);
    Task<VerificationToken?> FindByCodeAndUserIdAsync(string code, string userId);
    Task DeleteByIdAsync(ObjectId id);
}
