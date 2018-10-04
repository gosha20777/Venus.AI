using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    public class IntentModificationRespone : BaseRespone
    {
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
        [JsonProperty("intentName")]
        public string IntentName { get; set; }
    }
}
