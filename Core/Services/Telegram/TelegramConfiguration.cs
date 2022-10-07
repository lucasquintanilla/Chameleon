namespace Core.Services.Telegram
{
    public class TelegramConfiguration
    {
        public const string SectionName = "Telegram";

        public string Token { get; set; }
        public string ChatId { get; set; }
    }
}
