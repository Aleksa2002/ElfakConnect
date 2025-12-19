using System;

namespace Server.Data;

public class MongoDBSettings
{
    public required string ConnectionURI { get; set; }
    public required string DatabaseName { get; set; }
}
