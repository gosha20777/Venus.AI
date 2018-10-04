using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    public class IntentList
    {
        [JsonProperty("intents")]
        public List<string> Intents { get; set; } = new List<string>();
    }
}
