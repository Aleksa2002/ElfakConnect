using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Server.Data;

public static class MongoDBIndexInitializer
{
    public static void Initialize(IMongoDatabase database)
    {
        var userCollection = database.GetCollection<User>(User.CollectionName);

        var emailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true, Name = "unique_email" });

        var usernameIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Username),
            new CreateIndexOptions<User> { Unique = true, Sparse = true, Name = "unique_username" });

        var microsoftIdIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.MicrosoftId),
            new CreateIndexOptions<User> { Unique = true, Sparse = true, Name = "unique_microsoftId" });

        userCollection.Indexes.CreateMany([emailIndex, usernameIndex, microsoftIdIndex]);
    }
}

