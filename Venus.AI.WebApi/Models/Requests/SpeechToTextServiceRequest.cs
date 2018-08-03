using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public class SpeechToTextServiceRequest : VoiceRequest
    {
        [JsonProperty("language")]
        public string Language { get; set; }
    }
}
