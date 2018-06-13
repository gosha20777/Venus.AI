using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.Bot.Models
{
    public static class JsonConverter
    {
        public static string ToJson(ApiRequest messageContent)
        {
            if (messageContent == null)
                throw new Exception("Invalid IMessageContent!");

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(messageContent);
        }
        public static T FromJson<T>(string jsonString) where T : ApiRespone
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                throw new Exception("Can not convert from Json!");
            }

        }
        public static T AnyFromJson<T>(string jsonString) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch
            {
                throw new Exception("Can not convert from Json!");
            }
        }
    }
}
