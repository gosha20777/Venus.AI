using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.Bot.Models
{
    [JsonObject]
    public class ApiRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("requestType")]
        public string RequestType { get; set; }
        [JsonProperty("textData")]
        public string TextData { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
