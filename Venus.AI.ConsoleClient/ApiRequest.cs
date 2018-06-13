using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.ConsoleClient
{
    [JsonObject]
    public class ApiRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("requestType")]
        public string RequestType { get; set; }
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
        [JsonProperty("language")]
        public string Language{ get; set; }
    }
}
