using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services.Telegram
{
    public class TelegramConfiguration
    {
        public const string Telegram = "Telegram";

        public string Token { get; set; }
        public string ChatId { get; set; }
    }
}
