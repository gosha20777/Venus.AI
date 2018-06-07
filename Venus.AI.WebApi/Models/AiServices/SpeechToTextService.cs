using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class SpeechToTextService : IService
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
            return await InvorkGoogleSpeechApiAsync(voiceData);
        }

        private async Task<string> InvorkGoogleSpeechApiAsync(byte[] voiceData)
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
                var resStrings = jsonResult.Split("\"transcript\":\"");
                if (resStrings.Length > 1)
                    result = resStrings[1].Split('"').First();
                //TODO Throw Ex
            }
            return result;
        }
    }
}
