using System;
using MongoDB.Bson;
using Server.Common.Api;

namespace Server.Authentication.Services;

public interface IVerificationService
{
    Task<string> CreateCode(ObjectId userId);
    Task<Result<User>> VerifyUser(string userId, string code);
}
