namespace TgBotOrganaizer.Application
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    [ApiController]
    [Route("[controller]")]
    public class TgBotOrganaizerController : ControllerBase
    {
        private readonly ITelegramMessageHandler messageHandler;
        private readonly ITelegramBotClient botClient;

        public TgBotOrganaizerController(ITelegramBotClient botClient, ITelegramMessageHandler messageHandler)
        { 
            this.botClient = botClient;
            this.messageHandler = messageHandler;
        }

        //TODO: add function merge two different articles by /merge ~1~ ~2~
        [HttpPost]
        public async Task<IActionResult> PostTgMessage([FromBody] Update incomingMessage)
        {
            try
            {
                if (incomingMessage.Type is UpdateType.Message)
                {
                    
                    await this.messageHandler.HandleTelegramMessageAsync(incomingMessage.Message);
                }
            }
            catch (Exception e)
            {
                // Only for debug. Chat id can use only in controller layer. Filters dont get it
                await botClient.SendTextMessageAsync(
                    chatId: incomingMessage.Message.Chat.Id,
                    text: $"Произошла ошибка при отправке сообщения '{incomingMessage.Message.Text}'. Message={e.Message};StackTrace={e.StackTrace}");
            }
            
            return Ok();
        }
    }
}
