namespace TgBotOrganaizer.Application
{
    public class CommandConstants
    {
        public const string StartCommand = "/start";

        public const string GetAllThemesCommand = "/getallthemes";

        public const string GetArticleCommand = "/get";

        public const string InsertArticleCommand = "/insert";

        public const string IncomingMessagePattern = @"(?<command>\/get||\/insert||\/update)\s*~(?<theme>[^~]+)~\s*(?<text>.*)";
    }
}
