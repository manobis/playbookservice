using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using PlayBook.Service.Entities;

namespace PlayBook.Service.Repositories
{
    public class ItemsRepository
    {
        private const string collectionName = "items";
        private readonly IMongoCollection<Item> dbCollection;
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        public ItemsRepository()
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

            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase("Catalog");
            dbCollection = database.GetCollection<Item>(collectionName);
        }

        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<Item> GetAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Item entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(Item entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}