using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class RnnTalkSystemRespone : TextRespone
    {
        [JsonProperty("talkContext")]
        public string TalkContext { get; set; }
        [JsonProperty("talkReplicCount")]
        public int TalkReplicCount { get; set; }
    }
}
