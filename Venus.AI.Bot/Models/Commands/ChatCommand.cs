using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Venus.AI.Bot.Models.Commands
{
    public class ChatCommand : Command
    {
        public override string Name => "ChatCommand";

        public override bool Contains(Message message)
        {
            if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage 
                && !message.Text.StartsWith('/') 
                && !System.Text.RegularExpressions.Regex.IsMatch(message.Text, @"[^А-яЁё0-9\-!,.?\s]"))
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
                var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup
                {
                    Keyboard = new[]
                    {
                        new[] // row 1
                        {
                            new Telegram.Bot.Types.KeyboardButton("📝 исправить ответ 📝")
                        },
                    },
                    ResizeKeyboard = true
                };
                await client.SendTextMessageAsync(outputMessage.Id, outputMessage.OuputText, replyMarkup: keyboard);

                Replic inputR = new Replic()
                {
                    Id = message.Chat.Id,
                    Text = message.Text,
                    Time = message.Date
                };
                Replic outputR = new Replic()
                {
                    Id = 0,
                    Text = outputMessage.OuputText,
                    Time = message.Date
                };
                string[] lines = new string[]
                {
                    JsonConvert.SerializeObject(inputR),
                    JsonConvert.SerializeObject(outputR)
                };
                await System.IO.File.AppendAllLinesAsync($"{Environment.CurrentDirectory}\\{message.Chat.Id}.txt", lines);
            }
            catch (Exception ex)
            {
                Utils.Log.LogError(message.Chat.Id, ex.Message);
                await client.SendTextMessageAsync(message.Chat.Id, $"Произошла ошибка: `{ex.Message}`\nОбратитесь к @gosha20777", Telegram.Bot.Types.Enums.ParseMode.Markdown);
            }
        }
    }

    [JsonObject]
    public struct Replic
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public string Text { get; set; }
    }
}
