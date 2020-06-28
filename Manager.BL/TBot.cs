using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Manager.Common.Logger;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Linq;
using Manager.BL;

namespace ManagerBL
{
    public class TBot
    {
        private readonly TelegramBotClient _telegramBotClient;
        private readonly ILogger _logger;
        private string[] _userIdWhiteList;
        private CancellationTokenSource _cts;

        public event EventHandler BotStop;

        public TBot(string token, ILogger logger)
        {
            _logger = logger;

            try
            {
                _logger.Print(LogType.OK, "Bot initial . . .");
                _telegramBotClient = new TelegramBotClient(token);
                _telegramBotClient.OnMessage += _telegramBotClient_OnMessage;
            }
            catch (Exception ex)
            {
                _logger.Print(LogType.ERROR, ex.Message);
            }
        }

        public TBot(string token, ILogger logger, string[] userIdWhiteList): this(token, logger)
        {
            _userIdWhiteList = userIdWhiteList;
        }

        public async void StartAsync() 
        {
            _logger.Print(LogType.OK, "Bot is starting . . .");
            var me = await _telegramBotClient.GetMeAsync();
            if (me != null)
            {
                _logger.Print(LogType.SUCCESS, "Bot started succesfully");
                _cts = new CancellationTokenSource();

                UpdateType[] updateTypes = new UpdateType[1]
                {
                    UpdateType.Message
                };
                _telegramBotClient.StartReceiving(updateTypes, _cts.Token);
            }
        }

        public async Task Stop(Message message) 
        {
            await SendSimpleMessage(message, "Bot is stopping, good bye, Master");
            _logger.Print(LogType.OK, "Bot is stopping. . .");
            _telegramBotClient.StopReceiving();
            BotStop(this, null);
        }

        private async void _telegramBotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                if (CheckingSenderValidation(e.Message.From))
                {
                    _logger.Print(LogType.OK, $"Received message {e.Message.Text} from user {e.Message.From.Username} (id {e.Message.From.Id})");
                    await ParceIncomingMessage(e.Message);
                }
                else
                {
                    await SendSimpleMessage(e.Message, "Fuck you bitch, it's not your bot");
                }
            }
            catch (Exception ex)
            {
                _logger.Print(LogType.ERROR, $"Error with {e.Message.Text} from user {e.Message.From.Username} (id {e.Message.From.Id}): {ex.Message}");
            }
        }

        private async Task ParceIncomingMessage(Message message)
        {
            switch (message.Text)
            {
                case "/start":
                    await SendSimpleMessage(message, "What do you want, Master?");
                    break;
                case "/help":
                    await SendHelpMessage(message);
                    break;
                case "/shut_down":
                    await ShutDownPC(message);
                    break;
                case "/sleep":
                    await SleepPC(message);
                    break;
                case "/mute":
                    await MutePC(message);
                    break;
                case "/volume_up":
                    await UpVolumePC(message);
                    break;
                case "/volume_down":
                    await DownVolumePC(message);
                    break;
                case "/play_music":
                    await PlayMusicPC(message);
                    break;
                case "/stop":
                    await Stop(message);
                    break;
                default:
                    await SendSimpleMessage(message, $"I don't know command \"{message.Text}\", Master. \r\nI'm sorry.");
                    break;
            }
        }

        private bool CheckingSenderValidation(User from)
        {
            if (_userIdWhiteList == null || _userIdWhiteList.Length == 0)
                return true;
            
            return _userIdWhiteList.Contains(from.Id.ToString()) || _userIdWhiteList.Contains(from.Username);
        }

        private async Task SendSimpleMessage(Message message, string text)
        {
            try
            {
                await _telegramBotClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(500);
                await _telegramBotClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: text
                    );
                _logger.Print(LogType.OK, $"Message \"{text}\" was sent to user {message.From.Username} (id {message.From.Id})");
            }
            catch (Exception ex)
            {
                _logger.Print(LogType.ERROR, $"Error with sending essage \"{text}\" to user {message.From.Username} (id {message.From.Id}): {ex.Message}");
            }
        }

        private async Task SendHelpMessage(Message message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("I know this commands, Master:");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/help - my functions");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/stop - to stop me");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/shut_down - shut down your PC");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/sleep - put your computer to sleep mode");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/volume_up - up volume on your PC for 5%");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/volume_down - down volume on your PC for 5%");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/mute - turn on\\off sound on your PC");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("/play_music - turn on Yandex Radio");

            await SendSimpleMessage(message, stringBuilder.ToString());
        }

        private async Task ShutDownPC(Message message)
        {
            var result = PCManager.ShutDown(_logger);
            if (result)
                await SendSimpleMessage(message, "Your PC was shut down, Master");
            else
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

        private async Task SleepPC(Message message)
        {
            await SendSimpleMessage(message, "Your PC will go to sleep, Master");
            var result = PCManager.Sleep(_logger);
            if (!result)
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

        private async Task MutePC(Message message)
        {
            var result = PCManager.Mute(_logger);
            if (result)
                await SendSimpleMessage(message, "Your PC was mute\\unmute, Master");
            else
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

        private async Task UpVolumePC(Message message)
        {
            var result = PCManager.VolumeUp(_logger);
            if (result)
                await SendSimpleMessage(message, "Your PC volume was up for 5%, Master");
            else
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

        private async Task DownVolumePC(Message message)
        {
            var result = PCManager.VolumeDown(_logger);
            if (result)
                await SendSimpleMessage(message, "Your PC volume was down for 5%, Master");
            else
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

        private async Task PlayMusicPC(Message message)
        {
            var result = PCManager.Music(_logger);
            if (result)
                await SendSimpleMessage(message, "Yandex Radio was opened, Master");
            else
                await SendSimpleMessage(message, "We have some problems, Master...");
        }

    }
}
