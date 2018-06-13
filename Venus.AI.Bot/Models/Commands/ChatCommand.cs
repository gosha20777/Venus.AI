using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Venus.AI.Bot.Models.Commands
{
    public class ChatCommand : Command
    {
        public override string Name => "ChatCommand";

        public override bool Contains(Message message)
        {
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage && !message.Text.StartsWith('/'))
                return true;
            return false;
        }

        public override async Task Execute(Message message, TelegramBotClient client)
        {
            try
            {
                ApiRequest inputMessage = new ApiRequest()
                {
                    Id = message.Chat.Id,
                    Language = "rus",
                    RequestType = "text",
                    TextData = message.Text
                };
                var jsonRequest = JsonConverter.ToJson(inputMessage);
                var jsonRespone = RestApiClient.Post(jsonRequest);
                ApiRespone outputMessage = JsonConverter.FromJson<ApiRespone>(jsonRespone);
                await client.SendTextMessageAsync(outputMessage.Id, outputMessage.OuputText);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
