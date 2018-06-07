using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public string Invork(string inputText)
        {
            return _apiAi.Invork(inputText);
        }

        internal class ApiAiService : IService
        {
            private static ApiAiSDK.ApiAi apiAi;
            private static ApiAiSDK.RequestExtras requestExtras;
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
                requestExtras = new ApiAiSDK.RequestExtras();
            }

            public string Invork(string inputText)
            {
                ApiAiSDK.Model.AIResponse aiResponse;
                aiResponse = apiAi.TextRequest(inputText, requestExtras);

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

                var outputText = aiResponse.Result.Fulfillment.Speech;

                //DEBUG INFO
                #region DebugInfo
                Console.WriteLine("User: " + inputText);
                Console.WriteLine("Masha: " + outputText);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("BOT CONTEXTS:");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("# Paremetr Name");
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
                #endregion
                return outputText;
            }
        }
    }
}
