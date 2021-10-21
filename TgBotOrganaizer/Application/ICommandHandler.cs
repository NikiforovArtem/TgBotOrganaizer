using System;
using System.Threading.Tasks;
using Telegram.Bot;
using TgBotOrganaizer.Core.Entities;
using TgBotOrganaizer.Core.Interfaces;

namespace TgBotOrganaizer.Application
{
    public interface ICommandHandler
    {
        Task PostCommandHandlerAsync(string fileId, string photoCaption, long chatId, string text, string theme);
    }

    internal class CommandHandler : ICommandHandler
    {
        private readonly IArticleRepository articleRepository;
        private readonly ITelegramBotClient telegramBotClient;

        public CommandHandler(IArticleRepository articleRepository, ITelegramBotClient telegramBotClient)
        {
            this.articleRepository = articleRepository;
            this.telegramBotClient = telegramBotClient;
        }

        public async Task PostCommandHandlerAsync(string fileId, string photoCaption, long chatId, string text, string theme)
        {
            var article = await this.articleRepository.GetArticleByThemeAsync(theme);
            string callbackMessage;

            if (article != null)
            {
                await this.UpdateArticle(article, photoCaption, fileId, text);
                callbackMessage = "Тема обновлена";
            }
            else
            {
                await this.InsertArticle(photoCaption, fileId, text, theme);
                callbackMessage = "Тема сохранена";
            }

            await telegramBotClient.SendTextMessageAsync(
                chatId: chatId,
                text: callbackMessage);
        }

        private async Task UpdateArticle(Article article, string photoCaption, string fileId, string text)
        {
            if (!string.IsNullOrEmpty(fileId))
            {
                article.AddPhotoItem(photoCaption, fileId);
            }

            if (!string.IsNullOrEmpty(text))
            {
                article.AddText(text);
            }

            await this.articleRepository.UpdateAsync(article);
        }

        private async Task InsertArticle(string photoCaption, string fileId, string text, string theme)
        {
            // TODO: handle mediaGroupId. If message have 2 files then telegram send 2 different api requests with one mediaGroupId.
            var newArticleDto = new Article(theme, text, Guid.NewGuid().ToString());

            if (fileId != null)
            {
                newArticleDto.AddPhotoItem(photoCaption, fileId);
            }

            await this.articleRepository.InsertAsync(newArticleDto);
        }
    }
}
