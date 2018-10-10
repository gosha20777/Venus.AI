using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Services
{
    class SttService : IService
    {
        private YandexSttComponent _yandexStt;

        public void Initialize()
        {
            _yandexStt = new YandexSttComponent();
        }

        ServiceMessage IService.Invork(ServiceMessage message)
        {
            return InvorkAsync(message).Result;
        }

        public async System.Threading.Tasks.Task<ServiceMessage> InvorkAsync(ServiceMessage message)
        {
            VoiceMessage yaInMsg = new VoiceMessage()
            {
                Id = message.Id,
                Language = EnumsConvertor.StringToLanguage(message.Language),
                Vioce = message.Voice
            };
            var yaOutMsg = await _yandexStt.ProcessAsync(yaInMsg);
            message.Text = yaOutMsg.Text;
            return message;
        }
    }
}
