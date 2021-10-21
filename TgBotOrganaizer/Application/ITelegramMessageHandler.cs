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

            if (!match.Success)
            {
                await this.botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Неверная команда");

                return;
            }

            var command = match.Groups["command"];

            switch (command.Value)
            {
                case CommandConstants.StartCommand:
                    await this.getCommandHandler.HandleStartCommandAsync(chatId);
                    return;
                case CommandConstants.GetAllThemesCommand:
                    await this.getCommandHandler.HandleGetAllThemeCommandAsync(chatId);
                    return;
                case CommandConstants.GetArticleCommand:
                    await this.getCommandHandler.HandleGetCommandAsync(chatId, match.Groups["theme"].Value);
                    break;
                case CommandConstants.InsertArticleCommand:
                    await this.commandHandler.PostCommandHandlerAsync(
                        incomingMessage.Document?.FileId ?? incomingMessage.Photo?.FirstOrDefault()?.FileId,
                        incomingMessage.Caption,
                        chatId,
                        match.Groups["text"].Value,
                        match.Groups["theme"].Value);
                    break;
            }
        }
    }
}
