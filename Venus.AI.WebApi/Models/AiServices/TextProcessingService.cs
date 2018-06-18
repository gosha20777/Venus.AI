using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Respones;

namespace Venus.AI.WebApi.Models.AiServices
{
    public class TextProcessingService : IService
    {
        private ApiAiService _apiAi;
        public void Initialize(Enums.Language language)
        {
            _apiAi = new ApiAiService();
            _apiAi.Initialize(language);
        }

        public TextProcessingServiceRespone Invork(string inputText)
        {
            var respone = _apiAi.Invork(inputText);
            if (respone.IntentName == "input.unknown")
            {
                //TODO: Invork talk servise
                respone.IntentName = "none";
            }
            return respone;
        }

        private class ApiAiService : IService
        {
            private const bool SHOW_DEBUG_INFO = true;

            private static ApiAiSDK.ApiAi apiAi;
            private readonly string tocken = "768c62bfc2f04af2be7c85c47fabd3b2";

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
    }
}
