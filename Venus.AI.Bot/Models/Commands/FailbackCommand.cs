using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Venus.AI.Bot.Models.Commands
{
    public class FailbackCommand : Command
    {
        public override string Name => "FailBack command";

        public override bool Contains(Message message)
        {
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage
                && !message.Text.StartsWith('/')
                && System.Text.RegularExpressions.Regex.IsMatch(message.Text, @"[^А-яЁё0-9\-!,.?\s]"))
                return true;
            return false;
        }

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            await client.SendTextMessageAsync(message.Chat.Id, "Пожалуйста, старайтесь употреблять русский язык. Без смайликов :)");
        }
    }
}
