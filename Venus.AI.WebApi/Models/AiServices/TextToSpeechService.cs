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
        private readonly CancellationToken cancellationToken = new CancellationToken();

        public void Initialize(Enums.Language language)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> Invork(string text)
        {
            return await InvorkYandexSpeechKitAsync(text);
        }

        private async Task<byte[]> InvorkYandexSpeechKitAsync(string text)
        {
            byte[] speechData;
            var apiSetttings = new SpeechKitClientOptions("b10e4ccd-e306-4725-a1c0-75b447ef79f2", "MashaWebApi", Guid.Empty, "server");
            using (var client = new SpeechKitClient(apiSetttings))
            {
                var options = new SynthesisOptions(text, 1.3)
                {
                    AudioFormat = SynthesisAudioFormat.Wav,
                    Language = SynthesisLanguage.Russian,
                    Emotion = Emotion.Good,
                    Quality = SynthesisQuality.High,
                    Speaker = Speaker.Omazh
                };

                using (var textToSpechResult = await client.TextToSpeechAsync(options, cancellationToken).ConfigureAwait(false))
                {
                    if (textToSpechResult.TransportStatus != TransportStatus.Ok || textToSpechResult.ResponseCode != HttpStatusCode.OK)
                    {
                        throw new Exception("YandexSpeechKit error");
                    }
                    speechData = textToSpechResult.Result.ToByteArray();
                }
            }
            return speechData;
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
