using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components.Configurations;
using Venus.AI.SDK.Components.Exceptions;
using Venus.AI.SDK.Components.Messages;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Components
{
    class DialogueFlowComponent
    {
        private static ApiAiSDK.ApiAi apiAi;
        private readonly string tocken = $"{DialogueFlowConfig.ApiAiKey}";

        public TextMessage Process(TextMessage message, ref string userContext, out string intentName)
        {
            ApiAiSDK.AIConfiguration config;
            switch (message.Language)
            {
                case Language.English:
                    config = new ApiAiSDK.AIConfiguration(tocken, ApiAiSDK.SupportedLanguage.English);
                    break;
                case Language.Russian:
                    config = new ApiAiSDK.AIConfiguration(tocken, ApiAiSDK.SupportedLanguage.Russian);
                    break;
                default:
                    throw new Exceptions.InvalidMessageException(message.Id, "Invalid Language: " + message.Language.ToString());
            }
            apiAi = new ApiAiSDK.ApiAi(config);
            ApiAiSDK.Model.AIResponse aiResponse;
            ApiAiSDK.RequestExtras requestExtras;
            if (!string.IsNullOrWhiteSpace(userContext))
            {
                try
                {
                    requestExtras = JsonConvert.DeserializeObject<ApiAiSDK.RequestExtras>(userContext);
                }
                catch
                {
                    requestExtras = new ApiAiSDK.RequestExtras();
                }
            } 
            else
                requestExtras = new ApiAiSDK.RequestExtras();
            aiResponse = apiAi.TextRequest(message.Text, requestExtras);
            if (aiResponse == null)
                throw new InvalidMessageException(message.Id, "invalid ApiAiReapone");
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
            userContext = JsonConvert.SerializeObject(requestExtras);
            message.Text = aiResponse.Result.Fulfillment.Speech;
            intentName = aiResponse.Result.Action;
            if (string.IsNullOrWhiteSpace(intentName))
                intentName = "none";
            return message;
        }
    }
}
