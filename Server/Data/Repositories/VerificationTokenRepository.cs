using System;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Data.Interfaces;

namespace Server.Data.Repositories;

public class VerificationTokenRepository(IMongoDatabase database) : IVerificationTokenRepository
{
    private readonly IMongoCollection<VerificationToken> verificationTokenCollection = 
        database.GetCollection<VerificationToken>(VerificationToken.CollectionName);
    public async Task<VerificationToken> CreateAsync(ObjectId userId, string code, DateTime expiresAtUtc)
    {
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = SHA256.HashData(bytes);
        var token = new VerificationToken
        {
            UserId = userId,
            ExpiresAtUtc = expiresAtUtc,
            CodeHash = Convert.ToBase64String(hash)
        };
        await verificationTokenCollection.InsertOneAsync(token);
        
        return token;
    }
    public async Task<VerificationToken?> FindByCodeAndUserIdAsync(string code, string userId)
    {
        var bytes = Encoding.UTF8.GetBytes(code);
        var hash = SHA256.HashData(bytes);
        var codeHash = Convert.ToBase64String(hash);

        return await verificationTokenCollection
            .Find(t => t.CodeHash == codeHash &&
                        t.UserId == ObjectId.Parse(userId))
            .FirstOrDefaultAsync();
    }
    public async Task DeleteByIdAsync(ObjectId id)
    {
        await verificationTokenCollection.DeleteOneAsync(t => t.Id == id);
    }
}
