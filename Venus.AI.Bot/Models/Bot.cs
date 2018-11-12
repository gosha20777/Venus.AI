using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Venus.AI.Bot.Models.Commands;

namespace Venus.AI.Bot.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;

        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public static TelegramBotClient GetBotClient()
        {
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<Command>();
            commandsList.Add(new StartCommand());
            commandsList.Add(new ChatCommand());
            commandsList.Add(new FailbackCommand());
            //TODO: Add more commands

            botClient = new TelegramBotClient(AppSettings.Key);
            string hook = string.Format(AppSettings.Url, "api/message");
            Console.WriteLine($"BotKey: {AppSettings.Key}\n BotName: {AppSettings.Name}\nBotWebHook: {hook}");
            botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}
