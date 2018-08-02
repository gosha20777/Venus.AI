using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;
using Venus.AI.WebApi.Models.Utils;
using Newtonsoft.Json;
using ITCC.YandexSpeechKitClient;
using ITCC.YandexSpeechKitClient.Enums;
using System.Threading;
using NAudio.Wave;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class SpeechToTextService : BaseSpeechToTextService
    {
        private GoogleSpeechService _googleSpeechService;
        //private YandexSpeechKitService _yandexSpeechKitServiceService;
        //private WaweNetService _waweNetService;
        public override void Initialize(Enums.Language language)
        {
            _googleSpeechService = new GoogleSpeechService();
            _googleSpeechService.Initialize(language);

            //_yandexSpeechKitServiceService = new GoogleSpeechService();
            //_yandexSpeechKitServiceService.Initialize(language);
            //_waweNetService = new WaweNetService();
            //_waweNetService.Initialize(language);
        }

        public override async Task<TextRespone> Invork(VoiceRequest request)
        {
            var time = DateTime.Now;

            //AudioConvertor
            byte[] testSequence = request.VoiceData;
            using (WaveFileWriter writer = new WaveFileWriter($"{request.Id}.wav", new WaveFormat(16000, 1)))
            {
                writer.Write(testSequence, 0, testSequence.Length);
            }
            request.VoiceData = File.ReadAllBytes($"{request.Id}.wav");
            File.Delete($"{request.Id}.wav");
            
            var respone = await _googleSpeechService.Invork(request);
            Log.LogInformation(request.Id.Value, 0, this.GetType().ToString(), $"service end work in {(DateTime.Now - time).Milliseconds} ms");
            return respone;
        }


        #region GoogleSpeechService
        private class GoogleSpeechService : BaseSpeechToTextService
        {
            private string _language;
            public override void Initialize(Enums.Language language)
            {
                switch (language)
                {
                    case Enums.Language.English:
                        this._language = "en-US";
                        break;
                    case Enums.Language.Russian:
                        this._language = "ru-RU";
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
            }

            public override async Task<TextRespone> Invork(VoiceRequest request)
            {
                var time = DateTime.Now;

                string result = string.Empty;
                WebRequest webRequest = WebRequest.Create($"https://www.google.com/speech-api/v2/recognize?output=json&lang={_language}&key={AppConfig.GoogleSpeechApiKey}");
                webRequest.Method = "POST";
                webRequest.ContentType = "audio/l16; rate=16000"; //"16000";
                webRequest.ContentLength = request.VoiceData.Length;
                await webRequest.GetRequestStream().WriteAsync(request.VoiceData, 0, request.VoiceData.Length);
                HttpWebResponse webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    var jsonResult = await streamReader.ReadToEndAsync();
                    try
                    {
                        var resStrings = jsonResult.Split("\"transcript\":\"");
                        if (resStrings.Length > 1)
                            result = resStrings[1].Split('"').First();
                    }
                    catch { throw new Exceptions.InvalidTextDataException(); }
                    finally { if (string.IsNullOrWhiteSpace(result)) throw new Exceptions.InvalidTextDataException(); }
                }
                TextRespone respone = new TextRespone()
                {
                    Id = request.Id.Value,
                    TextData = result
                };

                Log.LogInformation(request.Id.Value, 0, this.GetType().ToString(), $"service end work in {(DateTime.Now - time).Milliseconds} ms");

                return respone;
            }
        }
        #endregion

        #region YandexSpeechKitService
        private class YandexSpeechKitService : IService
        {
            private RecognitionLanguage _language;
            private CancellationToken cancellationToken;

            public void Initialize(Enums.Language language)
            {
                switch (language)
                {
                    case Enums.Language.English:
                        this._language = RecognitionLanguage.English;
                        break;
                    case Enums.Language.Russian:
                        this._language = RecognitionLanguage.Russian;
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
            }
            public async Task<string> Invork(byte[] voiceData)
            {
                var apiSetttings = new SpeechKitClientOptions($"{AppConfig.YandexSpeechApiKey}", "MashaWebApi", Guid.Empty, "server");

                using (var client = new SpeechKitClient(apiSetttings))
                {
                    var speechRecognitionOptions = new SpeechRecognitionOptions(SpeechModel.Queries, RecognitionAudioFormat.Pcm16K, _language);
                    try
                    {
                        Stream mediaStream = new MemoryStream(voiceData);
                        var result = await client.SpeechToTextAsync(speechRecognitionOptions, mediaStream, cancellationToken).ConfigureAwait(false);
                        if (result.TransportStatus != TransportStatus.Ok || result.StatusCode != HttpStatusCode.OK)
                        {
                            throw new Exception("YandexSpeechKit error: " + result.TransportStatus.ToString());
                        }

                        if (!result.Result.Success)
                        {
                            throw new Exception("Unable to recognize speech"); 
                        }

                        Console.WriteLine(result);

                        var utterances = result.Result.Variants;
                        //Use recognition results
                        return utterances.First().Text;

                    }
                    catch (OperationCanceledException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region WaweNetService
        private class WaweNetService : IService
        {
            private string _language;
            public void Initialize(Enums.Language language)
            {
                switch (language)
                {
                    case Enums.Language.English:
                        this._language = "eng";
                        break;
                    case Enums.Language.Russian:
                        this._language = "rus";
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
            }

            public async Task<SpeechToTextServiceRespone> Invork(byte[] voiceData)
            {
                RestApiClient client = new RestApiClient("http://192.168.88.66:5000/");
                SpeechToTextServiceRequest request = new SpeechToTextServiceRequest
                {
                    Language = _language,
                    VoicehData = voiceData
                };
                if (client.HostActive())
                {
                    string jsonResp = await client.PostAsync(JsonConvert.SerializeObject(request));
                    return JsonConvert.DeserializeObject<SpeechToTextServiceRespone>(jsonResp);
                }
                else
                {
                    SpeechToTextServiceRespone respone = new SpeechToTextServiceRespone();
                    respone.Text = "unll";
                    respone.SucsessProbabitity = 0;
                    return respone;
                }
            }
        }
        #endregion
    }
}
