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

namespace Venus.AI.WebApi.Models.AiServices
{
    public class SpeechToTextService : IService
    {
        private YandexSpeechKitService _yandexSpeechKitServiceService;
        private GoogleSpeechService _googleSpeechService;
        //private WaweNetService _waweNetService;
        public void Initialize(Enums.Language language)
        {
            _yandexSpeechKitServiceService = new YandexSpeechKitService();
            _yandexSpeechKitServiceService.Initialize(language);
            _googleSpeechService = new GoogleSpeechService();
            _googleSpeechService.Initialize(language);

            //_waweNetService = new WaweNetService();
            //_waweNetService.Initialize(language);
        }

        public async Task<string> Invork(byte[] voiceData)
        {
            return await _googleSpeechService.Invork(voiceData);
            //return await _yandexSpeechKitServiceService.Invork(voiceData);
        }


        #region GoogleSpeechService
        private class GoogleSpeechService : IService
        {
            private string _language;
            public void Initialize(Enums.Language language)
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
            public async Task<string> Invork(byte[] voiceData)
            {
                string result = string.Empty;
                WebRequest request = WebRequest.Create($"https://www.google.com/speech-api/v2/recognize?output=json&lang={_language}&key=AIzaSyBOti4mM-6x9WDnZIjIeyEU21OpBXqWBgw");
                request.Method = "POST";
                request.ContentType = "audio/l16; rate=16000"; //"16000";
                request.ContentLength = voiceData.Length;
                await request.GetRequestStream().WriteAsync(voiceData, 0, voiceData.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(response.GetResponseStream()))
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
                return result;
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
                var apiSetttings = new SpeechKitClientOptions("4f2562b1-7519-413f-b3ae-17b52789e3ae", "MashaWebApi", Guid.Empty, "server");

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
                RestApiClient.Сonfigure("http://192.168.88.66:5000/");
                SpeechToTextServiceRequest request = new SpeechToTextServiceRequest
                {
                    Language = _language,
                    VoicehData = voiceData
                };
                if (RestApiClient.Connect())
                {
                    string jsonResp = await RestApiClient.PostAsync(JsonConvert.SerializeObject(request));
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
