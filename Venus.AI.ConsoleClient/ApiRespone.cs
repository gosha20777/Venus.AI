using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.ConsoleClient
{
    [JsonObject]
    public class ApiRespone
    {
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
        [JsonProperty("ouputText")]
        public string OuputText { get; set; }
        [JsonProperty("intentName")]
        public string IntentName { get; set; }
        [JsonProperty("entities")]
        public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
    }
}
