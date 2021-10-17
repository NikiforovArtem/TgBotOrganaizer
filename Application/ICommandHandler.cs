using System.Linq;
using Telegram.Bot;
using TgBotOrganaizer.Core.Entities;
using TgBotOrganaizer.Core.Interfaces;

namespace TgBotOrganaizer.Application
{
    using System.Threading.Tasks;

    public interface IGetCommandHandler
    {
        Task HandleCommandAsync(long chatId, string theme);
    }

    internal class GetCommandHandler : IGetCommandHandler
    {
        private readonly IArticleRepository articleRepository;
        private readonly ITelegramBotClient botClient;

        public GetCommandHandler(IArticleRepository articleRepository)
        {
            this.articleRepository = articleRepository;
        }

        public async Task HandleCommandAsync(long chatId, string theme)
        {
            var article = await this.articleRepository.GetArticleByThemeAsync(theme);

            if (article.PhotoItems != null && article.PhotoItems.Any())
            {
                foreach (var photo in article.PhotoItems)
                {
                    await this.botClient.SendPhotoAsync(chatId, photo.ExternalId);
                }
            }

            if (string.IsNullOrEmpty(article.Text))
            {
                return;
            }

            await this.botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{article.Text}");
        }
    }
}
