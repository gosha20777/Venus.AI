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

namespace Venus.AI.WebApi.Models.AiServices
{
    public class SpeechToTextService : IService
    {
        private GoogleSpeechService _googleSpeechService;
        private WaweNetService _waweNetService;
        public void Initialize(Enums.Language language)
        {
            _googleSpeechService = new GoogleSpeechService();
            _googleSpeechService.Initialize(language);

            _waweNetService = new WaweNetService();
            _waweNetService.Initialize(language);
        }

        public async Task<string> Invork(byte[] voiceData)
        {
            var respone = await _waweNetService.Invork(voiceData);
            if(respone.SucsessProbabitity < 0.8)
                return await _googleSpeechService.Invork(voiceData);
            return respone.Text;
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
                SpeechToTextServiceRespone respone = new SpeechToTextServiceRespone();
                respone.Text = "1";
                respone.SucsessProbabitity = 0;
                return respone;
                /*
                RestApiClient.Сonfigure("http://192.168.88.66:5000/");
                SpeechToTextServiceRequest request = new SpeechToTextServiceRequest
                {
                    Language = _language,
                    VoicehData = voiceData
                };
                if (RestApiClient.Connect())
                {
                    Console.WriteLine("SENDING!!!!!!");
                    string jsonResp = await RestApiClient.PostAsync(JsonConvert.SerializeObject(request));
                    Console.WriteLine(jsonResp);
                    return JsonConvert.DeserializeObject<SpeechToTextServiceRespone>(jsonResp);
                }
                else
                {
                    SpeechToTextServiceRespone respone = new SpeechToTextServiceRespone();
                    respone.Text = "1";
                    respone.SucsessProbabitity = 0;
                    return respone;
                }
                */
            }
        }
        #endregion
    }
}
