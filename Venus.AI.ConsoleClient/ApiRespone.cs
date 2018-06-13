using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.ConsoleClient
{
    [JsonObject]
    public class ApiRespone
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
        [JsonProperty("ouputText")]
        public string OuputText { get; set; }
    }
}
