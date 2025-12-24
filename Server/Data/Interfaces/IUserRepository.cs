using System;
using MongoDB.Bson;
using Server.Common;

namespace Server.Data.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> CreateAsync(string userName, string email, string passwordHash);
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByIdAsync(ObjectId userId);
    Task<User?> FindByEmailAsync(string email);
    Task<Result<User>> VerifyByIdAsync(string userId);
}
