using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.Bot.Models
{
    [JsonObject]
    public class ApiRespone
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("ouputText")]
        public string OuputText { get; set; }
    }
}
