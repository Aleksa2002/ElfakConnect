using System;
using MongoDB.Bson;
using Server.Common;

namespace Server.Authentication.Interfaces;

public interface IVerificationService
{
    Task<string> CreateCode(ObjectId userId);
    Task<Result<User>> VerifyUser(string userId, string code);
}
