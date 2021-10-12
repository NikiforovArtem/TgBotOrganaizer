using MongoDB.Driver;
using TgBotOrganaizer.Core.Entities.SeedWork;

namespace TgBotOrganaizer.InfrastructureMongo
{
    public interface IMongoRepository<TDocument> where TDocument : IAggregateRoot
    {
    }

    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IAggregateRoot
    {
        protected readonly IMongoCollection<TDocument> collection;

        public MongoRepository(IDotNetTheoryMongoStoreSettings settings)
        {
            var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
            this.collection = database.GetCollection<TDocument>(nameof(TDocument));
        }
    }
}
