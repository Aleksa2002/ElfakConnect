using System;
using MongoDB.Bson;

namespace Server.Data.Models;

public class VerificationToken : IOwnedEntity
{
    public ObjectId Id { get; set ; }
    public required string CodeHash { get; set; }
    public required ObjectId UserId { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public static string CollectionName => "verification_tokens";
}
