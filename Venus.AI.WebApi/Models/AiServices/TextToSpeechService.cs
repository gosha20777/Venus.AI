using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class TextToSpeechService : BaseTextToSpeechService
    {
        private YandexSpeechKitService _yandexSpeechKitService;
        public override void Initialize(Enums.Language language)
        {
            _yandexSpeechKitService = new YandexSpeechKitService();
            _yandexSpeechKitService.Initialize(language);
        }

        public override async Task<VoiceRespone> Invork(TextRequest textRequest)
        {
            return await _yandexSpeechKitService.Invork(textRequest);
        }

        private class YandexSpeechKitService : BaseTextToSpeechService
        {
            private readonly CancellationToken cancellationToken = new CancellationToken();
            private SynthesisLanguage _language;
            
            public override void Initialize(Enums.Language language)
            {
                switch (language)
                {
                    case Enums.Language.English:
                        this._language = SynthesisLanguage.English;
                        break;
                    case Enums.Language.Russian:
                        this._language = SynthesisLanguage.Russian;
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
            }
            public override async Task<VoiceRespone> Invork(TextRequest textRequest)
            {
                VoiceRespone voiceRespone = new VoiceRespone() { Id = textRequest.Id.Value };
                var apiSetttings = new SpeechKitClientOptions($"{AppConfig.YandexSpeechApiKey}", "MashaWebApi", Guid.Empty, "server");
                using (var client = new SpeechKitClient(apiSetttings))
                {
                    var options = new SynthesisOptions(textRequest.TextData, 1.1)
                    {
                        AudioFormat = SynthesisAudioFormat.Wav,
                        Language    = _language,
                        Emotion     = Emotion.Good,
                        Quality     = SynthesisQuality.High,
                        Speaker     = Speaker.Omazh
                    };

                    using (var textToSpechResult = await client.TextToSpeechAsync(options, cancellationToken).ConfigureAwait(false))
                    {
                        if (textToSpechResult.TransportStatus != TransportStatus.Ok || textToSpechResult.ResponseCode != HttpStatusCode.OK)
                        {
                            throw new Exception("YandexSpeechKit error: " + textToSpechResult.ResponseCode.ToString());
                        }
                        voiceRespone.VoiceData = textToSpechResult.Result.ToByteArray();
                    }
                }
                return voiceRespone;
            }
        }
    }

    /// <summary>
    /// Extensions for Stream class
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Converts stream to byew array
        /// </summary>
        /// <param name="stream">input stream</param>
        /// <returns>byte array</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
