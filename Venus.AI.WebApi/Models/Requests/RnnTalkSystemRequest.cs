using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public class RnnTalkSystemRequest : TextRequest
    {
        [JsonProperty("talkContext")]
        public string TalkContext { get; set; }
        [JsonProperty("talkReplicCount")]
        public int TalkReplicCount { get; set; }
    }
}
