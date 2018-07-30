using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public abstract class BaseRequest
    {
        private long _id;
        [JsonProperty("id")]
        public long? Id
        {
            get { return _id; }
            set
            {
                if (value != null && value >= 0)
                    _id = value.Value;
                else
                    throw new InvalidIdExcrption();
            }
        }
    }
}
