using System;
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
            new CreateIndexOptions { Unique = true, Name = "unique_username" });

        userCollection.Indexes.CreateMany([emailIndex, usernameIndex]);
    }
}

