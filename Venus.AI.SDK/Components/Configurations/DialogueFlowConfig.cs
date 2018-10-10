using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Components.Configurations
{
    [JsonObject]
    class DialogueFlowConfig
    {
        [JsonProperty("apiAiKey")]
        public static string ApiAiKey { get; set; }
    }
}
