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
using Venus.AI.WebApi.Models.Requests;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class TextProcessingService : BaseTextProcessingService
    {
        private ApiAiService _apiAi;
        private RnnTalkService _rnnTalkService;
        public override void Initialize(Enums.Language language)
        {
            _apiAi = new ApiAiService();
            _apiAi.Initialize(language);

            _rnnTalkService = new RnnTalkService();
            _rnnTalkService.Initialize(language);
        }

        public override async Task<TextProcessingServiceRespone> Invork(TextRequest textRequest)
        {
            var respone = await _apiAi.Invork(textRequest);
            respone.Id = textRequest.Id.Value;
            if (respone.IntentName == "input.unknown")
            {
                respone = await _rnnTalkService.Invork(textRequest);
            }
            Console.WriteLine("User> {0}", textRequest.TextData);
            Console.WriteLine("Venus.AI> {0}", respone.TextData);
            return respone;
        }

        #region DialogFlow
        private class ApiAiService : BaseTextProcessingService
        {
            private const bool SHOW_DEBUG_INFO = true;

            private static ApiAiSDK.ApiAi apiAi;
            private readonly string tocken = $"{AppConfig.ApiAiKey}";

            public override void Initialize(Enums.Language language)
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

            public override async Task<TextProcessingServiceRespone> Invork(TextRequest textRequest)
            {
                TextProcessingServiceRespone textProcessingServiceRespone = new TextProcessingServiceRespone() { Id = textRequest.Id.Value };
                ApiAiSDK.Model.AIResponse aiResponse;
                var requestExtras = new ApiAiSDK.RequestExtras();
                aiResponse = apiAi.TextRequest(textRequest.TextData, StsticContext.GetContext(textRequest.Id.Value));

                //TODO: Update Exceptions
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
                StsticContext.SetContext(requestExtras, textRequest.Id.Value);
                requestExtras = null;
                textProcessingServiceRespone.TextData = aiResponse.Result.Fulfillment.Speech;


                if (!string.IsNullOrWhiteSpace(aiResponse.Result.Action))
                {
                    foreach (var parametr in aiResponse.Result.Parameters)
                    {
                        if (!string.IsNullOrWhiteSpace(parametr.Value.ToString()))
                        {
                            textProcessingServiceRespone.Entities.Add(parametr.Key, parametr.Value.ToString());
                            if (parametr.Value.ToString().Contains("dress"))
                                textProcessingServiceRespone.WayPoint = "adidas";
                            else if(parametr.Value.ToString().Contains("furniture"))
                                textProcessingServiceRespone.WayPoint = "ikea";
                            else if (parametr.Value.ToString().ToLower().Contains("ikea"))
                                textProcessingServiceRespone.WayPoint = "ikea";
                            else if (parametr.Value.ToString().ToLower().Contains("adidas"))
                                textProcessingServiceRespone.WayPoint = "adidas";
                        }
                            
                    }
                    textProcessingServiceRespone.IntentName = aiResponse.Result.Action;
                }
                else
                    textProcessingServiceRespone.IntentName = "none";

                //DEBUG INFO
                #region DebugInfo
                if (SHOW_DEBUG_INFO)
                {
                    Console.WriteLine("Id: {0} | BOT PARAMS:", textRequest.Id.Value);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Intent {0}", textProcessingServiceRespone.IntentName);
                    Console.WriteLine("\tName\t\t\t|Value");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    foreach (var parametr in textProcessingServiceRespone.Entities)
                    {
                        Console.WriteLine("\t{0,-23} |{1}", parametr.Key, parametr.Value);
                    }
                    Console.WriteLine(textProcessingServiceRespone.WayPoint);
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
        private class RnnTalkService : BaseTextProcessingService
        {
            private Enums.Language _language;
            public override void Initialize(Enums.Language language)
            {
                this._language = language;
            }

            public override async Task<TextProcessingServiceRespone> Invork(TextRequest textRequest)
            {
                using (RabbitMqClient client = new RabbitMqClient("localhost"))
                {
                    string inputQueue = "RnnTalkService", outputQueue = "RnnTalkService";
                    TextProcessingServiceRespone respone = new TextProcessingServiceRespone
                    {
                        IntentName = "none",
                        TextData = ""
                    };
                    if (_language == Enums.Language.English)
                    {
                        inputQueue += "_input_en";
                        outputQueue += "_output_en";
                    }
                    else if (_language == Enums.Language.Russian)
                    {
                        inputQueue += "_input_ru";
                        outputQueue += "_output_ru";
                    }

                    //TODO: replase RnnTalkServiceMessage to TextRequest
                    RnnTalkServiceMessage message = new RnnTalkServiceMessage()
                    {
                        TextData = textRequest.TextData
                    };
                    string responeRnn;
                    responeRnn = await client.PostAsync(JsonConvert.SerializeObject(message), inputQueue, outputQueue);
                    message = JsonConvert.DeserializeObject<RnnTalkServiceMessage>(responeRnn);

                    respone.TextData = message.TextData;
                    return respone;
                }
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
                        + $"key={AppConfig.YandexTranslatorKey}"
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

                            foreach (string str in translation.Text)
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
                public string Code { get; set; }
                public string Lang { get; set; }
                public string[] Text { get; set; }
            }
        }
        #endregion
    }
}
