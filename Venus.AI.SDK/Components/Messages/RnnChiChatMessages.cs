using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Components.Messages
{
    [JsonObject]
    public class RnnChiChatMessage
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("textData")]
        public string TextData { get; set; }
        [JsonProperty("talkContext")]
        public string TalkContext { get; set; }
        [JsonProperty("talkReplicCount")]
        public int TalkReplicCount { get; set; }
    }
}
