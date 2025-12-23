using System;
using MongoDB.Bson;

namespace Server.Data.Models;

public class RefreshToken : IOwnedEntity
{
    public ObjectId Id { get; set; }
    public required string TokenHash { get; set; }
    public required ObjectId UserId { get; set; }
    public bool IsRevoked { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public static string CollectionName => "refresh_tokens";
}
