using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class TextProcessingServiceRespone : TextRespone
    {
        [JsonProperty("id")]
        public new long Id { get; set; }
        [JsonProperty("textData")]
        public new string TextData { get; set; }
        [JsonProperty("intentName")]
        public string IntentName { get; set; }
        [JsonProperty("wayPoint")]
        public string WayPoint { get; set; }
        [JsonProperty("entities")]
        public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
    }
}
