using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public class VoiceRequest : BaseRequest
    {
        private byte[] _voiceData;
        [JsonProperty("voiceData")]
        public byte[] VoiceData
        {
            get { return _voiceData; }
            set
            {
                if (value != null && value.Any())
                    _voiceData = value;
                else
                    throw new ApiRequestException(Id, new InvalidVoiceDataException());
            }
        }
    }
}
