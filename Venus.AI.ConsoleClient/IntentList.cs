using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.ConsoleClient
{
    public class IntentList
    {
        [JsonProperty("intents")]
        public List<string> Intents { get; set; } = new List<string>();
    }
}
