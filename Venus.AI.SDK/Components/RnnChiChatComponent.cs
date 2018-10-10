using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Clients;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Components
{
    class RnnChiChatComponent
    {
        public RnnChiChatMessage Post(TextMessage message, string userContext, int replicCount)
        {
            return ProcessAsync(message, userContext, replicCount).Result;
        }

        public async Task<RnnChiChatMessage> ProcessAsync(TextMessage message, string userContext, int replicCount)
        {
            using (RabbitMqClient client = new RabbitMqClient("localhost"))
            {
                string inputQueue = "RnnTalkService", outputQueue = "RnnTalkService";
                switch (message.Language)
                {
                    case Language.English:
                        inputQueue += "_input_en";
                        outputQueue += "_output_en";
                        break;
                    case Language.Russian:
                        inputQueue += "_input_ru";
                        outputQueue += "_output_ru";
                        break;
                    default:
                        throw new Exceptions.InvalidMessageException(message.Id, "Invalid Language: " + message.Language.ToString());
                }

                RnnChiChatMessage talkSystemRequest = new RnnChiChatMessage();
                talkSystemRequest.Id = message.Id;
                talkSystemRequest.TextData = message.Text;
                talkSystemRequest.TalkContext = userContext;
                talkSystemRequest.TalkReplicCount = replicCount;
                string responeRnn;
                responeRnn = await client.PostAsync(JsonConvert.SerializeObject(talkSystemRequest), inputQueue, outputQueue);
                var talkSystemRespone = JsonConvert.DeserializeObject<RnnChiChatMessage>(responeRnn);
                return talkSystemRespone;
            }
        }
    }
}
