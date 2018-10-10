using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Services
{
    class TtsService : IService
    {
        private YandexTtsComponent _yandexTts;

        public void Initialize()
        {
            _yandexTts = new YandexTtsComponent();
        }

        public ServiceMessage Invork(ServiceMessage message)
        {
            return InvorkAsync(message).Result;
        }

        public async System.Threading.Tasks.Task<ServiceMessage> InvorkAsync(ServiceMessage message)
        {
            TextMessage yaInMsg = new TextMessage()
            {
                Id = message.Id,
                Language = EnumsConvertor.StringToLanguage(message.Language),
                Text = message.Text
            };
            var yaOutMsg = await _yandexTts.ProcessAsync(yaInMsg);
            message.Voice = yaOutMsg.Vioce;
            return message;
        }
    }
}
