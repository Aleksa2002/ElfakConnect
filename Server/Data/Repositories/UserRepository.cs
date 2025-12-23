using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Server.Common.Api;
using Server.Users;

namespace Server.Data.Repositories;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<User> _usersCollection = database.GetCollection<User>(User.CollectionName);

    public async Task<Result<User>> CreateAsync(string userName, string email, string passwordHash)
    {
        try
        {
            var user = new User
            {
                Username = userName,
                Email = email,
                PasswordHash = passwordHash
            };

            await _usersCollection.InsertOneAsync(user);
            return user;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            if (ex.Message.Contains("email"))
            {
                return UserErrors.EmailNotUnique(email);
            }
            if (ex.Message.Contains("username"))
            {
                return UserErrors.UsernameNotUnique(userName);
            }
            throw;
        }
    }

    public async Task<User?> FindByIdAsync(string userId)
    {
        return await _usersCollection
            .Find(u => u.Id == ObjectId.Parse(userId))
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindByIdAsync(ObjectId userId)
    {
        return await _usersCollection
            .Find(u => u.Id == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _usersCollection
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<Result<User>> VerifyByIdAsync(string userId)
    {
        var updatedDoc = await _usersCollection.FindOneAndUpdateAsync(
            u => u.Id == ObjectId.Parse(userId),
            Builders<User>.Update.Set(u => u.IsVerified, true),
            new FindOneAndUpdateOptions<User>
            {
                ReturnDocument = ReturnDocument.After // Returns the document after update
            });

        if (updatedDoc == null)
        {
            return UserErrors.UserNotFound(userId.ToString());
        }
        return updatedDoc;
    }
}
