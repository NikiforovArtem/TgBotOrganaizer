﻿using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotOrganaizer.Core.Interfaces;

namespace TgBotOrganaizer.Application
{
    using System.Threading.Tasks;

    public interface IGetCommandHandler
    {
        Task HandleGetCommandAsync(long chatId, string theme);

        Task HandleGetAllThemeCommandAsync(long chatId);

        Task HandleStartCommandAsync(long chatId);
    }

    internal class GetCommandHandler : IGetCommandHandler
    {
        private readonly IArticleRepository articleRepository;
        private readonly ITelegramBotClient botClient;

        public GetCommandHandler(IArticleRepository articleRepository, ITelegramBotClient botClient)
        {
            this.articleRepository = articleRepository;
            this.botClient = botClient;
        }

        public async Task HandleGetCommandAsync(long chatId, string theme)
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
