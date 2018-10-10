using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Services
{
    class TextProcessService : IService
    {
        private DialogueFlowComponent _dialogueFlow;
        private RnnChiChatComponent _rnnChiChat;

        public void Initialize()
        {
            _dialogueFlow = new DialogueFlowComponent();
            _rnnChiChat = new RnnChiChatComponent();
        }

        ServiceMessage IService.Invork(ServiceMessage message)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task<ServiceMessage> InvorkAsync(ServiceMessage message)
        {
            TextMessage textMessage = new TextMessage
            {
                Id = message.Id,
                Language = EnumsConvertor.StringToLanguage(message.Language),
                Text = message.Text
            };

            string userApiAiContext = "", userRnnContext = "";

            textMessage = _dialogueFlow.Process(textMessage, ref userApiAiContext, out string intentName);
            if (intentName == "input.unknown")
            {
                RnnChiChatMessage chiChatMessage = await _rnnChiChat.ProcessAsync(textMessage, userRnnContext, 0);
                message.Text = chiChatMessage.TextData;
            }
            return message;
        }
    }
}
