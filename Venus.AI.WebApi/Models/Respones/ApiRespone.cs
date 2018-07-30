using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class ApiRespone : BaseRespone
    {
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
        [JsonProperty("ouputText")]
        public string OuputText { get; set; }
        [JsonProperty("intentName")]
        public string IntentName { get; set; }
        [JsonProperty("entities")]
        public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
        [JsonProperty("wayPoint")]
        public string WayPoint { get; set; }
    }
}
