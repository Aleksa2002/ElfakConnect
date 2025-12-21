using System;
using MongoDB.Bson;

namespace Server.Data.Models;

public interface IEntity
{
    public ObjectId Id { get; set; }
    static abstract string CollectionName { get; }
}

public interface IOwnedEntity : IEntity
{
    public ObjectId UserId { get; set; }
}