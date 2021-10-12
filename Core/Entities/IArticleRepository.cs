using System.Collections.Generic;
using System.Threading.Tasks;
using TgBotOrganaizer.Core.Entities.SeedWork;

namespace TgBotOrganaizer.Core.Entities
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<Article> GetArticleByThemeAsync(string theme);

        Task<string> InsertAsync(Article newArticle);

        Task<string> UpdateAsync(Article article);

        Task<List<string>> GetAllThemes();
    }
}