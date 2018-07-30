using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Respones
{
    [JsonObject]
    public abstract class BaseRespone
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }
}
