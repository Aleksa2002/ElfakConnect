using System;
using MongoDB.Driver;

namespace Server.Data;

public static class MongoDBServiceExtensions
{
    public static IServiceCollection AddMongoDB(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var mongoSettings = configuration.GetSection("MongoDB").Get<MongoDBOptions>()
            ?? throw new InvalidOperationException("MongoDb settings not found in configuration");

        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(mongoSettings.ConnectionURI);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });

        return services;
    }
}
