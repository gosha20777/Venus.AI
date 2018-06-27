using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Respones;
using Venus.AI.WebApi.Models.Utils;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class TextProcessingService : IService
    {
        private ApiAiService _apiAi;
        private RnnTalkService _rnnTalkService;
        public void Initialize(Enums.Language language)
        {
            _apiAi = new ApiAiService();
            _apiAi.Initialize(language);

            _rnnTalkService = new RnnTalkService();
            _rnnTalkService.Initialize(language);
        }

        public TextProcessingServiceRespone Invork(string inputText)
        {
            var respone = _apiAi.Invork(inputText);
            if (respone.IntentName == "input.unknown")
            {
                respone = _rnnTalkService.Invork(inputText);
            }
            Console.WriteLine("User> {0}", inputText);
            Console.WriteLine("Venus.AI> {0}", respone.OutputText);
            return respone;
        }

        #region DialogFlow
        private class ApiAiService : IService
        {
            private const bool SHOW_DEBUG_INFO = true;

            private static ApiAiSDK.ApiAi apiAi;
            private readonly string tocken = "b753a131dba74e5d8fe0fe2d60cd4b2b";

            public void Initialize(Enums.Language language)
            {
                ApiAiSDK.AIConfiguration config;
                switch (language)
                {
                    case Enums.Language.English:
                        config = new ApiAiSDK.AIConfiguration(tocken, ApiAiSDK.SupportedLanguage.English);
                        break;
                    case Enums.Language.Russian:
                        config = new ApiAiSDK.AIConfiguration(tocken, ApiAiSDK.SupportedLanguage.Russian);
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
                apiAi = new ApiAiSDK.ApiAi(config);
            }

            public TextProcessingServiceRespone Invork(string inputText)
            {
                TextProcessingServiceRespone textProcessingServiceRespone = new TextProcessingServiceRespone();
                ApiAiSDK.Model.AIResponse aiResponse;
                var requestExtras = new ApiAiSDK.RequestExtras();
                aiResponse = apiAi.TextRequest(inputText, StsticContext.GetContext());

                if (aiResponse == null)
                    throw new Exception("Invalid output message");

                requestExtras.Contexts = new List<ApiAiSDK.Model.AIContext>();
                foreach (var outContext in aiResponse.Result.Contexts)
                {
                    ApiAiSDK.Model.AIContext aIContext = new ApiAiSDK.Model.AIContext
                    {
                        Parameters = new Dictionary<string, string>(),
                        Lifespan = outContext.Lifespan,
                        Name = outContext.Name
                    };
                    foreach (var param in outContext.Parameters)
                    {
                        string key = param.Key;
                        string value;
                        if (param.Value != null)
                        {
                            value = param.Value.ToString();
                            aIContext.Parameters.Add(key, value);
                        }
                    }
                    requestExtras.Contexts.Add(aIContext);
                }
                StsticContext.SetContext(requestExtras);
                requestExtras = null;
                textProcessingServiceRespone.OutputText = aiResponse.Result.Fulfillment.Speech;


                if (!string.IsNullOrWhiteSpace(aiResponse.Result.Action))
                {
                    foreach (var parametr in aiResponse.Result.Parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parametr.Value.ToString()))
                            textProcessingServiceRespone.Entities.Add(parametr.Key, parametr.Value.ToString());
                    }
                    textProcessingServiceRespone.IntentName = aiResponse.Result.Action;
                }
                else
                    textProcessingServiceRespone.IntentName = "none";

                //DEBUG INFO
                #region DebugInfo
                if (SHOW_DEBUG_INFO)
                {
                    Console.WriteLine("BOT PARAMS:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Intent {0}", textProcessingServiceRespone.IntentName);
                    Console.WriteLine("\tName\t\t\t|Value");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    foreach (var parametr in textProcessingServiceRespone.Entities)
                    {
                        Console.WriteLine("\t{0,-23} |{1}", parametr.Key, parametr.Value);
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    /*
                    Console.WriteLine("User: " + inputText);
                    Console.WriteLine("Masha: " + outputText);
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("BOT CONTEXTS:");
                    foreach (var context in aiResponse.Result.Contexts)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("## {0}", context.Name);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("\tName\t\t\t|Value");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        foreach (var parameter in context.Parameters)
                            Console.WriteLine("\t{0,-23} |{1}", parameter.Key, parameter.Value);
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("BOT PARAMS:");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("\tName\t\t\t|Value");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    foreach (var parametr in aiResponse.Result.Parameters)
                    {
                        Console.WriteLine("\t{0,-23} |{1}", parametr.Key, parametr.Value);
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                    */
                }

                #endregion
                return textProcessingServiceRespone;
            }
        }
        #endregion
        #region RnnTalkService
        private class RnnTalkService : IService
        {
            private Enums.Language _language;
            public void Initialize(Enums.Language language)
            {
                this._language = language;
            }

            public TextProcessingServiceRespone Invork(string inputText)
            {
                TextProcessingServiceRespone respone = new TextProcessingServiceRespone
                {
                    IntentName = "none",
                    OutputText = ""
                };

                if (_language == Enums.Language.Russian)
                {
                    inputText = TextTranslator.Translate(inputText, Enums.Language.Russian, Enums.Language.English);
                }
                
                RestApiClient.Сonfigure("http://192.168.88.66:5000/");
                RnnTalkServiceMessage message = new RnnTalkServiceMessage()
                {
                    TextData = inputText
                };
                string responeRnn = RestApiClient.Post(JsonConvert.SerializeObject(message));
                message = JsonConvert.DeserializeObject<RnnTalkServiceMessage>(responeRnn);
                Console.WriteLine(">>>" + message.TextData);
                if (_language == Enums.Language.Russian)
                    message.TextData = TextTranslator.Translate(message.TextData, Enums.Language.English, Enums.Language.Russian);
                respone.OutputText = message.TextData;

                return respone;
            }
            [JsonObject]
            private class RnnTalkServiceMessage
            {
                [JsonProperty("textData")]
                public string TextData { get; set; }
            }
        }

        private static class TextTranslator
        {
            public static string Translate(string inputText, Enums.Language inputLanguage, Enums.Language outputLanguage)
            {
                string inputLanguageStr = GetLanguage(inputLanguage);
                string outputLanguageStr = GetLanguage(outputLanguage);

                string result = Translate(inputText, inputLanguageStr, outputLanguageStr);
                return result;
            }

            //word - фраза для перевода, SL -исходный язык, DL - язык перевода.
            private static string Translate(string word, string SL, string DL)
            {
                if (word.Length > 0)
                {
                    string lang = SL + "-" + DL;
                    WebRequest request = WebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/translate?"
                        + "key=trnsl.1.1.20180627T133735Z.619c9117248591b0.209b898c7425c156cc8461d105966542f3dd0629"
                        + "&text=" + word
                        + "&lang=" + lang);

                    WebResponse response = request.GetResponse();

                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        string line;

                        if ((line = stream.ReadLine()) != null)
                        {
                            Translation translation = JsonConvert.DeserializeObject<Translation>(line);

                            word = "";

                            foreach (string str in translation.text)
                            {
                                word += str;
                            }
                        }
                    }

                    return word;
                }
                else
                    return "";
            }
            private static string GetLanguage(Enums.Language language)
            {
                string languageStr;
                switch (language)
                {
                    case Enums.Language.English:
                        languageStr = "en";
                        break;
                    case Enums.Language.Russian:
                        languageStr = "ru";
                        break;
                    default:
                        throw new Exceptions.InvalidLanguageException(language.ToString());
                }
                return languageStr;
            }

            class Translation
            {
                public string code { get; set; }
                public string lang { get; set; }
                public string[] text { get; set; }
            }
        }
        #endregion
    }
}
