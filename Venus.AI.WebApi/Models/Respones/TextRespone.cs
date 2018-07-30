using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public class TextRespone : BaseRespone
    {
        [JsonProperty("textData")]
        public string TextData { get; set; }
    }
}
