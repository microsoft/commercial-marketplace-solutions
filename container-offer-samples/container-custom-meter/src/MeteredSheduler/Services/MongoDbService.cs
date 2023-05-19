using MeteredSheduler.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MeteredSheduler.Services
{

    
    public class MongoDbService
    {
        private readonly IMongoCollection<Item> _itemCollection;
        public MongoDbService()
        {
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL"); ;
            var databaseName = Environment.GetEnvironmentVariable("DATABASE_NAME"); ;
            var collection = Environment.GetEnvironmentVariable("DATABASE_COLLECTION"); ;


            var mongoClient = new MongoClient(connectionString);

            var mongoDatabase = mongoClient.GetDatabase(databaseName);

            _itemCollection = mongoDatabase.GetCollection<Item>(collection);
        }

        public async Task<List<Item>> GetAsync() =>
        await _itemCollection.Find(_ => true).ToListAsync();

        public async Task<List<Item>> GetQueuedItemAsync(string dimName) =>
        await _itemCollection.Find(x => x.MeterProcessStatus==false && x.DimensionId==dimName).ToListAsync();


        public async Task<Item?> GetAsync(string id) =>
            await _itemCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Item newItem) =>
            await _itemCollection.InsertOneAsync(newItem);

        public async Task UpdateAsync(string id, Item updatedItem) =>
            await _itemCollection.ReplaceOneAsync(x => x.Id == id, updatedItem);

        public async Task RemoveAsync(string id) =>
            await _itemCollection.DeleteOneAsync(x => x.Id == id);
    }
}
