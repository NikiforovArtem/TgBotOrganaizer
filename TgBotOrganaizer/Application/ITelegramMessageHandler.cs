using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TgBotOrganaizer.Application
{
    public interface ITelegramMessageHandler
    {
        Task HandleTelegramMessageAsync(Message incomingMessage);
    }

    internal class TelegramMessageHandler : ITelegramMessageHandler
    {
        private readonly ITelegramBotClient botClient;
        private readonly IQueryHandler getCommandHandler;
        private readonly ICommandHandler commandHandler;

        public TelegramMessageHandler(ITelegramBotClient botClient, IQueryHandler getCommandHandler, ICommandHandler commandHandler)
        {
            this.botClient = botClient;
            this.getCommandHandler = getCommandHandler;
            this.commandHandler = commandHandler;
        }

        public async Task HandleTelegramMessageAsync(Message incomingMessage)
        {
            // TODO: Validate by message type. not caption or text
            var messageText = incomingMessage.Text ?? incomingMessage.Caption;
            var chatId = incomingMessage.Chat.Id;

            var regex = new Regex(CommandConstants.IncomingMessagePattern, RegexOptions.Singleline);

            var match = regex.Match(messageText);
            var command = match.Groups["command"];

            if (!match.Success || string.IsNullOrEmpty(command.Value))
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Неверная команда");

                return;
            }

            switch (command.Value)
            {
                case CommandConstants.StartCommand:
                    await this.getCommandHandler.HandleStartCommandAsync(chatId);
                    return;
                case CommandConstants.GetAllThemesCommand:
                    await this.getCommandHandler.HandleGetAllThemeCommandAsync(chatId);
                    return;
                case CommandConstants.GetArticleCommand:
                    var themeValueGet = match.Groups["theme"].Value;

                    if (!await this.ValidateTheme(themeValueGet, chatId))
                    {
                        return;
                    }

                    await this.getCommandHandler.HandleGetCommandAsync(chatId, themeValueGet);
                    return;
                case CommandConstants.InsertArticleCommand:
                    var themeValueInsert = match.Groups["theme"].Value;
                    var textValueInsert = match.Groups["text"].Value;

                    if (!await this.ValidateTheme(themeValueInsert, chatId) && !await this.ValidateText(textValueInsert, chatId))
                    {
                        return;
                    }

                    await this.commandHandler.PostCommandHandlerAsync(
                        incomingMessage.Document?.FileId ?? incomingMessage.Photo?.FirstOrDefault()?.FileId,
                        incomingMessage.Caption,
                        chatId,
                        textValueInsert,
                        themeValueInsert);
                   return;
            }
        }

        private async Task<bool> ValidateTheme(string theme, long chatId)
        {
            if (string.IsNullOrEmpty(theme))
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Не задана тема");

                return false;
            }

            return true;
        }

        private async Task<bool> ValidateText(string text, long chatId)
        {
            if (string.IsNullOrEmpty(text))
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Не введён текст статьи");

                return false;
            }

            return true;
        }
    }
}