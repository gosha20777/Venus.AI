using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models
{
    [JsonObject]
    public class AppConfig
    {
        [JsonProperty("googleSpeechApiKey")]
        public static string GoogleSpeechApiKey { get; set; }
        [JsonProperty("apiAiKey")]
        public static string ApiAiKey { get; set; }
        [JsonProperty("yandexSpeechApiKey")]
        public static string YandexSpeechApiKey { get; set; }
        [JsonProperty("yandexTranslatorKey")]
        public static string YandexTranslatorKey { get; set; }
        [JsonProperty("rnnTalkServiceUrl_en")]
        public static string RnnTalkServiceUrl { get; set; }
        [JsonProperty("rnnTalkServiceUrl_ru")]
        public static string RnnTalkServiceUrlRu { get; set; }
        [JsonProperty("connectionString")]
        public static string CinnectionString { get; set; }
    }
}
