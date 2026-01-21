using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Data.Models;

public class User : IEntity
{
    public ObjectId Id { get; set; }
    public required string Email { get; set; }
    [BsonIgnoreIfNull]
    public string? Username { get; set; }
    public string? PasswordHash { get; set; }
    [BsonIgnoreIfNull]
    public string? MicrosoftId { get; set; }
    public bool IsVerified { get; set; } = false;
    public bool IsOnboarded {get; set;} = false;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public static string CollectionName => "users";
}

