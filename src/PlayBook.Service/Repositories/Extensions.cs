using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PlayBook.Service.Entities;
using PlayBook.Service.Settings;

namespace PlayBook.Service.Repositories
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            // Tells MongoDB to automatically treat all C# Guids as standard Bson BinaryData subtypes
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            // Register IMongoDatabase using the passed-in 'services' instance instead of 'builder.Services'
            services.AddSingleton(serviceProvider =>
            {
                // Use GetRequiredService to safely resolve the IConfiguration dependency
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()
                    ?? throw new InvalidOperationException($"{nameof(MongoDbSettings)} section is missing from appsettings.json");

                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

                if (serviceSettings == null)
                {
                    throw new InvalidOperationException($"{nameof(ServiceSettings)} must be initialized before registering MongoDB.");
                }

                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            // An extension method extending IServiceCollection must return the collection instance
            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}
