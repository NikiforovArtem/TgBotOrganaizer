using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TgBotOrganaizer.Core.Entities;

namespace TgBotOrganaizer.InfrastructureMongo
{
    internal class ArticleMongoRepository : MongoRepository<Article>, IArticleRepository
    {
        public ArticleMongoRepository(IDotNetTheoryMongoStoreSettings settings) : base(settings)
        {
        }

        public async Task<Article> GetArticleByThemeAsync(string theme)
        {
            return await this.collection.FindAsync(a => a.Theme == theme).Result.FirstOrDefaultAsync();
        }

        public async Task<string> InsertAsync(Article article)
        {
            await this.collection.InsertOneAsync(article);
            return article.Id;
        }

        public async Task<string> UpdateAsync(Article article)
        {
            var filter = Builders<Article>.Filter.Eq(doc => doc.Id, article.Id);
            await this.collection.FindOneAndReplaceAsync(filter, article);
            return article.Id;
        }

        public async Task<List<string>> GetAllThemes()
        {
            var filter = await this.collection.Find(new BsonDocument()).Project(a => a.Theme).ToListAsync();
            return filter;
        }
    }
}