using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class VoiceRespone : BaseRespone
    {
        [JsonProperty("voiceData")]
        public byte[] VoiceData { get; set; }
    }
}
