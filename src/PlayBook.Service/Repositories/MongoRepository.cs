using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using PlayBook.Service.Entities;
using PlayBook.Service.Repositories;

namespace PlayBook.Service.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            // Containerization: 
            // MongoDB running in Docker Desktop, maps its ports to your Windows localhost @ 27017.

            //## Docker ##
            // Run mongo db in docker
            // Create Mongo db in container: 
            // docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo 
            // (Note: -rm flag destroys container each time project is closed and powered back on.)

            // Create Mongo db in container: 
            // docker run -d --name mongo -p 27017:27017 -v mongodbdata:/data/db mongo
            // (Note: no -rm flag in this. The container stays and not destroyed.)

            // Upon dependency injection and configuration System, the below two commented
            // out lines are no more necessary. The settings in handled in the appsettings.json 
            // file under the name "MongoDbSettings" and "ServiceSettings".

            // var mongoClient = new MongoClient("mongodb://localhost:27017");
            // var database = mongoClient.GetDatabase("Catalog");

            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}