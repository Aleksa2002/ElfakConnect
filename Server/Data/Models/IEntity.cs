using System;
using MongoDB.Bson;

namespace Server.Data.Models;

public interface IEntity
{
    public ObjectId Id { get; set; }
}

public interface IOwnedEntity
{
    public ObjectId UserId { get; set; }
}