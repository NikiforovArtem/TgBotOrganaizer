using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotOrganaizer.Core.Entities;
using TgBotOrganaizer.Core.Interfaces;

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
        private readonly IGetCommandHandler getCommandHandler;

        public TelegramMessageHandler(ITelegramBotClient botClient, IArticleRepository articleRepository, IGetCommandHandler getCommandHandler)
        {
            this.botClient = botClient;
            this.articleRepository = articleRepository;
            this.getCommandHandler = getCommandHandler;
        }

        public async Task HandleTelegramMessageAsync(Message incomingMessage)
        {
            // TODO: Validate by message type. not caption or text
            if (string.IsNullOrWhiteSpace(incomingMessage.Text) && string.IsNullOrWhiteSpace(incomingMessage.Caption))
            {
                return;
            }

            var messageText = incomingMessage.Text ?? incomingMessage.Caption;
            var chatId = incomingMessage.Chat.Id;

            var regex = new Regex(CommandConstants.IncomingMessagePattern, RegexOptions.Singleline);

            var match = regex.Match(messageText);
            var command = match.Groups["command"];

            if (!match.Success || string.IsNullOrWhiteSpace(command.Value))
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Неверная команда");

                return;
            }


            //TODO: Add caption to photo entity

            if (messageText == CommandConstants.StartCommand)
            {
                await this.getCommandHandler.HandleStartCommandAsync(chatId);
                return;
            }

            if (messageText == CommandConstants.GetAllThemesCommand)
            {
                await this.getCommandHandler.HandleGetAllThemeCommandAsync(chatId);
                return;
            }


            switch (command.Value.ToLower())
            {
                case CommandConstants.GetArticleCommand:
                    await this.getCommandHandler.HandleGetCommandAsync(chatId, match.Groups["theme"].Value);
                    break;
                case CommandConstants.InsertArticleCommand:
                    await this.HandleInsertOrUpdateCommandAsync(match, incomingMessage.Document?.FileId ?? incomingMessage.Photo?.FirstOrDefault()?.FileId, incomingMessage.Caption, chatId); 
                    break;
            }
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

            if (fileId != null)
            {
                newArticleDto.AddPhotoItem(photoCaption, fileId);
            }

            await this.articleRepository.InsertAsync(newArticleDto);

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Тема сохранена");
        }
    }
}
