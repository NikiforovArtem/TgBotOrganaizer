using MongoDB.Driver;
using TgBotOrganaizer.Core.Entities.SeedWork;

namespace TgBotOrganaizer.Infrastructure.InfrastructureMongo
{
    internal class MongoRepository<TDocument> where TDocument : IAggregateRoot
    {
        protected readonly IMongoCollection<TDocument> collection;

        protected MongoRepository(string connectionString, string databaseName, string collectionName)
        {
            var database = new MongoClient(connectionString).GetDatabase(databaseName);
            this.collection = database.GetCollection<TDocument>(collectionName);
        }
    }
}
