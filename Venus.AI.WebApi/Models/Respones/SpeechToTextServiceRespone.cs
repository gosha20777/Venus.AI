using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class SpeechToTextServiceRespone
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("sucsessProbabitity")]
        public double SucsessProbabitity { get; set; }
    }
}
