using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class TextToSpeechService : IService
    {
        private YandexSpeechKitService _yandexSpeechKitService;
        public void Initialize(Enums.Language language)
        {
            _yandexSpeechKitService = new YandexSpeechKitService();
            _yandexSpeechKitService.Initialize(language);
        }

        public async Task<byte[]> Invork(string text)
        {
            return await _yandexSpeechKitService.Invork(text);
        }

        private class YandexSpeechKitService : IService
        {
            private readonly CancellationToken cancellationToken = new CancellationToken();
            private SynthesisLanguage _language;
            
            public void Initialize(Enums.Language language)
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
            public async Task<byte[]> Invork(string text)
            {
                byte[] speechData;
                var apiSetttings = new SpeechKitClientOptions("4f2562b1-7519-413f-b3ae-17b52789e3ae", "MashaWebApi", Guid.Empty, "server");
                using (var client = new SpeechKitClient(apiSetttings))
                {
                    var options = new SynthesisOptions(text, 1.3)
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
                        speechData = textToSpechResult.Result.ToByteArray();
                    }
                }
                return speechData;
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
