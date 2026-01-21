using System;
using MongoDB.Bson;
using Server.Common;

namespace Server.Data.Interfaces;

public interface IUserRepository
{
    Task<Result<User>> CreateAsync(
        string email,
        string? username = null,
        string? passwordHash = null,
        string? microsoftId = null,
        bool isVerified = false);
    Task<User?> FindByIdAsync(string userId);
    Task<User?> FindByIdAsync(ObjectId userId);
    Task<User?> FindByMicrosoftIdAsync(string microsoftId);
    Task<User?> FindByEmailAsync(string email);
    Task<Result<User>> VerifyByIdAsync(string userId);
}
