using System;
using MongoDB.Bson;

namespace Server.Data.Models;

public class User : IEntity
{

    public ObjectId Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public static string CollectionName => "users";
}

