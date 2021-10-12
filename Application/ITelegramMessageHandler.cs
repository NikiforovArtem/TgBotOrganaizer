using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotOrganaizer.Core.Entities;

namespace TgBotOrganaizer.Application
{
    public interface ITelegramMessageHandler
    {
        Task HandleTelegramMessageAsync(Message incomingMessage);
    }

    internal class TelegramMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramBotClient botClient;
        private readonly IArticleRepository articleRepository;

        public TelegramMessageHandler(ITelegramBotClient botClient, IArticleRepository articleRepository)
        {
            this.botClient = botClient;
            this.articleRepository = articleRepository;
        }

        // TODO: вынести команды в справочник с константами
        public async Task HandleTelegramMessageAsync(Message incomingMessage)
        {
            // TODO: Validate by message type. not caption or text
            if (string.IsNullOrEmpty(incomingMessage.Text) && string.IsNullOrEmpty(incomingMessage.Caption))
            {
                return;
            }

            var regex = new Regex(CommandConstants.IncomingMessagePattern, RegexOptions.Singleline);

            //TODO: Add caption to photo entity
            var messageText = incomingMessage.Text ?? incomingMessage.Caption;
            var chatId = incomingMessage.Chat.Id;

            if (messageText == CommandConstants.StartCommand)
            {
                await this.GetReplyButtons(chatId);
                return;
            }

            if (messageText == CommandConstants.GetAllThemesCommand)
            {
                var allThemes = await this.articleRepository.GetAllThemes();

                //TODO: сделать кнопками inline с колбэком сразу на get по теме
                foreach (var theme in allThemes)
                {
                    await this.botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: theme);
                }

                return;
            }

            var match = regex.Match(messageText);

            if (!match.Success)
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Неверная команда");

                return;
            }

            var command = match.Groups["command"];

            // TODO: Add more handlers by command and with fabric from DI
            switch (command.Value.ToLower())
            {
                case CommandConstants.GetArticleCommand:
                    await this.HandleGetCommandAsync(match, chatId);
                    break;
                case CommandConstants.InsertArticleCommand:
                    await this.HandleInsertOrUpdateCommandAsync(match, incomingMessage.Document?.FileId ?? incomingMessage.Photo?.FirstOrDefault()?.FileId, incomingMessage.Caption, chatId); 
                    break;
            }
        }

        private async Task GetReplyButtons(long chatId)
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

        private async Task HandleGetCommandAsync(Match incomingMessageMatch, long chatId)
        {
            var theme = incomingMessageMatch.Groups["theme"];
            var article = await this.articleRepository.GetArticleByThemeAsync(theme.Value);
            
            //TODO: вынести в слой инфраструктуры взаимодействие с API Telegram
            if (article.PhotoItems != null && article.PhotoItems.Any())
            {
                foreach (var photo in article.PhotoItems)
                {
                    await botClient.SendPhotoAsync(chatId, photo.ExternalId);
                }
            }

            if (string.IsNullOrEmpty(article.Text))
            {
                return;
            }

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"{article.Text}");
        }

        private async Task HandleInsertOrUpdateCommandAsync(Match incomingMessageMatch, string fileId, string photoCaption, long chatId)
        {
            var theme = incomingMessageMatch.Groups["theme"];

            // TODO: handle large text. Telegram send 2 different api request when > 3000 symbols
            var text = incomingMessageMatch.Groups["text"];

            var article = await this.articleRepository.GetArticleByThemeAsync(theme.Value);

            if (article != null)
            {
                if (!string.IsNullOrEmpty(fileId))
                {
                    article.AddPhotoItem(photoCaption, fileId);
                }

                if (!string.IsNullOrEmpty(text.Value))
                {
                    article.AddText(text.Value);
                }

                await this.articleRepository.UpdateAsync(article);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Тема обновлена");

                return;
            }

            // TODO: handle mediaGroupId. If message have 2 files then telegram send 2 different api requests with one mediaGroupId.
            var newArticleDto = new Article(theme.Value, text.Value, Guid.NewGuid().ToString());
            newArticleDto.AddPhotoItem("", fileId);
            await this.articleRepository.InsertAsync(newArticleDto);

            //TODO: засылать картинки просто по одному запросу,обновляя просто статью, а в caption указывать тему, к которой относится картинка

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Тема сохранена");
        }
    }
}
