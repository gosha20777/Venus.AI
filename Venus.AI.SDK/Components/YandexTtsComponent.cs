using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Venus.AI.SDK.Components.Configurations;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Structures;

namespace Venus.AI.SDK.Components
{
    class YandexTtsComponent : BaseTtsComponent
    {
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private SynthesisLanguage _language;

        public override VoiceMessage Process(TextMessage message)
        {
            return ProcessAsync(message).Result;
        }

        public async System.Threading.Tasks.Task<VoiceMessage> ProcessAsync(TextMessage message)
        {
            switch (message.Language)
            {
                case Core.Enums.Language.English:
                    this._language = SynthesisLanguage.English;
                    break;
                case Core.Enums.Language.Russian:
                    this._language = SynthesisLanguage.Russian;
                    break;
                default:
                    throw new Exceptions.InvalidMessageException(message.Id, "Invalid Language: " + message.Language.ToString());
            }
            var apiSetttings = new SpeechKitClientOptions($"{YandexTtsCompmnentConfig.YandexSpeechApiKey}", "MashaWebApi", Guid.Empty, "server");
            using (var client = new SpeechKitClient(apiSetttings))
            {
                var options = new SynthesisOptions(message.Text, YandexTtsCompmnentConfig.Speed)
                {
                    AudioFormat = SynthesisAudioFormat.Wav,
                    Language = _language,
                    Emotion = Emotion.Good,
                    Quality = SynthesisQuality.High,
                    Speaker = Speaker.Omazh
                };

                using (var textToSpechResult = await client.TextToSpeechAsync(options, cancellationToken).ConfigureAwait(false))
                {
                    if (textToSpechResult.TransportStatus != TransportStatus.Ok || textToSpechResult.ResponseCode != HttpStatusCode.OK)
                    {
                        throw new Exception("YandexSpeechKit error: " + textToSpechResult.ResponseCode.ToString());
                    }
                    VoiceMessage result = new VoiceMessage
                    {
                        Id = message.Id,
                        Language = message.Language,
                        Vioce = textToSpechResult.Result.ToByteArray()
                    };
                    return result;
                }
            }
        }
    }
}
