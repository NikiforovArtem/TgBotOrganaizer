namespace TgBotOrganaizer.Application
{
    public class CommandConstants
    {
        public const string StartCommand = "/start";

        public const string GetAllThemesCommand = "/getallthemes";

        public const string GetArticleCommand = "/get";

        public const string InsertArticleCommand = "/post";

        public const string IncomingMessagePattern = @"(?<command>\/post|\/getallthemes|\/start|\/get)((\s~(?<theme>[^~]+)~)?(\s*(?<text>.*)))";
    }
}