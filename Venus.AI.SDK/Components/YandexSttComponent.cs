using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Venus.AI.SDK.Components.Configurations;
using Venus.AI.SDK.Components.Messages;

namespace Venus.AI.SDK.Components
{
    public class YandexSttComponent : BaseSttComponent
    {
        private readonly CancellationToken cancellationToken = new CancellationToken();
        private RecognitionLanguage _language;

        public async System.Threading.Tasks.Task<TextMessage> ProcessAsync(VoiceMessage message)
        {
            switch (message.Language)
            {
                case Core.Enums.Language.English:
                    this._language = RecognitionLanguage.English;
                    break;
                case Core.Enums.Language.Russian:
                    this._language = RecognitionLanguage.Russian;
                    break;
                default:
                    throw new Exceptions.InvalidMessageException(message.Id, "Invalid Language: " + message.Language.ToString());
            }
            var apiSetttings = new SpeechKitClientOptions($"{YandexCompmnentConfig.YandexSpeechApiKey}", "MashaWebApi", Guid.Empty, "server");
            using (var client = new SpeechKitClient(apiSetttings))
            {
                MemoryStream mediaStream = new MemoryStream(message.Vioce);
                var speechRecognitionOptions = new SpeechRecognitionOptions(SpeechModel.Queries, RecognitionAudioFormat.Wav, RecognitionLanguage.Russian);
                try
                {
                    var result = await client.SpeechToTextAsync(speechRecognitionOptions, mediaStream, cancellationToken).ConfigureAwait(false);
                    if (result.TransportStatus != TransportStatus.Ok || result.StatusCode != HttpStatusCode.OK)
                    {
                        //Handle network and request parameters error
                    }

                    if (!result.Result.Success)
                    {
                        //Unable to recognize speech
                    }

                    var utterances = result.Result.Variants;
                    if (utterances.Count > 0)
                    {
                        var max = utterances[0];
                        foreach (var item in utterances)
                        {
                            if (item.Confidence > max.Confidence)
                                max = item;
                        }
                        TextMessage res = new TextMessage()
                        {
                            Id = message.Id,
                            Language = message.Language,
                            Text = max.Text
                        };
                        return res;
                    }
                    throw new Exception("invdlid answer");
                }
                catch (OperationCanceledException)
                {
                    throw new Exception("invdlid answer");
                }
            }
        }

        public override TextMessage Process(VoiceMessage message)
        {
            return ProcessAsync(message).Result;
        }
    }
}
