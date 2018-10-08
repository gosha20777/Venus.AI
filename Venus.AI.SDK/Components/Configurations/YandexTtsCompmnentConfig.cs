using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Venus.AI.SDK.Components.Configurations
{
    [JsonObject]
    public class YandexTtsCompmnentConfig
    {
        [JsonProperty("yandexSpeechApiKey")]
        public static string YandexSpeechApiKey { get; set; }
        [JsonProperty("yandexSpeechApiSpeed")]
        public static double Speed { get; set; } = 1.1;
        [JsonProperty("yandexSpeechApiSpeaker")]
        public static ITCC.YandexSpeechKitClient.Enums.Speaker Speaker { get; set; } = ITCC.YandexSpeechKitClient.Enums.Speaker.Omazh;
    }
}
