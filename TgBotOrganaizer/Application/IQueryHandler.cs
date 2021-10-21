using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotOrganaizer.Core.Interfaces;

namespace TgBotOrganaizer.Application
{
    using System.Threading.Tasks;

    public interface IQueryHandler
    {
        Task HandleGetCommandAsync(long chatId, string theme);

        Task HandleGetAllThemeCommandAsync(long chatId);

        Task HandleStartCommandAsync(long chatId);
    }

    internal class QueryHandler : IQueryHandler
    {
        private readonly IArticleRepository articleRepository;
        private readonly ITelegramBotClient botClient;

        public QueryHandler(IArticleRepository articleRepository, ITelegramBotClient botClient)
        {
            this.articleRepository = articleRepository;
            this.botClient = botClient;
        }

        public async Task HandleGetCommandAsync(long chatId, string theme)
        {
            var article = await this.articleRepository.GetArticleByThemeAsync(theme);

            if (article == null)
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Заданной темы не существует");

                return;
            }

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

            //TODO: separate if > 500 symbols
            await this.botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{article.Text}");
        }

        public async Task HandleGetAllThemeCommandAsync(long chatId)
        {
            var allThemes = await this.articleRepository.GetAllThemes();

            var markup = new ReplyKeyboardMarkup { Keyboard = allThemes.Select(theme => new[] { new KeyboardButton($"{CommandConstants.GetArticleCommand} ~{theme}~") }) };

            await this.botClient.SendTextMessageAsync(chatId, "Выберите тему", replyMarkup: markup);
        }

        public async Task HandleStartCommandAsync(long chatId)
        {
            var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
            {
                Keyboard = new[] {
                    new[]
                    {
                        new KeyboardButton(CommandConstants.GetAllThemesCommand),
                    },
                },
                ResizeKeyboard = true
            };

            await this.botClient.SendTextMessageAsync(chatId, "Команды...", replyMarkup: keyboard);
        }
    }
}
