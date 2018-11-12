using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.Bot.Models
{
    [JsonObject]
    public class AppSettings
    {
        [JsonProperty("url")]
        public static string Url { get; set; } = "https://URL:443/{0}";
        [JsonProperty("name")]
        public static string Name { get; set; } = "<BOT_NAME>";
        [JsonProperty("key")]
        public static string Key { get; set; } = "<BOT_KEYs>";
        [JsonProperty("venus_ai_url")]
        public static string VenusAiUri { get; set; } = "URL";
    }
}
