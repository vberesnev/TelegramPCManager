using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace ManagerBL
{
    class TBot
    {
        TelegramBotClient _telegramBotClient;

        public TBot(string token)
        {
            _telegramBotClient = new TelegramBotClient(token);

        }
        public void Start() { }

        public void Stop() { }
    }
}
